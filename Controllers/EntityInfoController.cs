using CodeFirstGenerator.Data;
using CodeFirstGenerator.Dtos;
using CodeFirstGenerator.Entities;
using ELF.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using RazorEngine;
using RazorEngine.Templating;

namespace CodeFirstGenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntityInfoController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        private readonly IGeneratorService generatorService;

        public EntityInfoController(ApplicationDbContext db, IGeneratorService generatorService)
        {
            this.db = db;
            this.generatorService = generatorService;
        }

        [HttpPost]
        public async Task<AmisResult> CreateAsync(EntityInfoImportInput input)
        {
            if (string.IsNullOrEmpty(input.Path))
            {
                throw new ArgumentException("请输入文件路径");
            }

            await db.EntityInfos.ToListAsync();

            //判断是文件夹路径还是文件路径
            if (input.Path.EndsWith(".cs"))
            {
                await CreateOrUpdateByFileAsync(input.Path, db.EntityInfos.Local);
            }
            else
            {
                // 遍历文件夹中的所有.cs文件
                var files = Directory.GetFiles(input.Path, "*.cs", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    await CreateOrUpdateByFileAsync(file, db.EntityInfos.Local);
                }
            }

            await db.SaveChangesAsync();

            return AmisResult.Ok();
        }

        private async Task CreateOrUpdateByFileAsync(string filePath, ICollection<EntityInfo> entityInfos)
        {
            // 检查文件是否存在
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("文件不存在");
            }

            // 读取文件内容
            var code = System.IO.File.ReadAllText(filePath);

            // 检查文件内容是否为空
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentException("文件内容为空");
            }

            // 解析代码
            var list = await generatorService.ParseClassCodeAsync(code);

            foreach (var item in list)
            {
                var entityInfo = entityInfos.FirstOrDefault(e => e.ClassName == item.ClassName);
                if (entityInfo != null)
                {
                    db.EntityInfos.Remove(entityInfo);
                    item.Folder = entityInfo.Folder;
                }
                else
                {
                    //根据filePath当前文件夹名称
                    var folder = Path.GetDirectoryName(filePath);
                    item.Folder = Path.GetFileName(folder!);
                }
                item.FilePath = filePath;

                item.OriginalCode = code;

                await db.EntityInfos.AddAsync(item);
            }
        }

        [HttpGet("{id}")]
        public async Task<AmisResult<EntityInfo>> GetAsync(long id)
        {
            var entityInfo = await db.EntityInfos
                .Include(x => x.Properties).ThenInclude(x => x.Attributes)
                .Include(x => x.Attributes)
                .FirstOrDefaultAsync(e => e.Id == id) ?? throw new Exception("实体信息未找到");

            return AmisResult.Ok(entityInfo);
        }

        [HttpGet]
        public async Task<AmisResult<AmisPagedResultDto<EntityInfo>>> GetListAsync([FromQuery] EntityInfoGetListInput input)
        {
            var query = db.EntityInfos.AsNoTracking()
                .WhereNotNull(input.Using, x => x.Using.Contains(input.Using!))
                .WhereNotNull(input.Namespace, x => x.Namespace.Contains(input.Namespace!))
                .WhereNotNull(input.ClassName, x => x.ClassName.Contains(input.ClassName!))
                .WhereNotNull(input.Description, x => x.Description.Contains(input.Description!))
                .WhereNotNull(input.FilePath, x => x.FilePath.Contains(input.FilePath!))
                .WhereNotNull(input.Folder, x => x.Folder.Contains(input.Folder!))
                ;
            var totalCount = await query.CountAsync();

            var entities = new List<EntityInfo>();

            if (totalCount > 0)
            {
                input.OrderBy = input.OrderBy ?? nameof(input.ClassName);
                query = query.AmisOrderBy(input);
                query = query.AmisPaging(input);

                entities = await query.ToListAsync();
            }

            return AmisResult.Ok(new AmisPagedResultDto<EntityInfo>(
                totalCount,
                entities
            ));
        }

        [HttpDelete("{id}")]
        public async Task<AmisResult> DeleteAsync(long id)
        {
            var entityInfo = await db.EntityInfos
                .Include(x => x.Properties).ThenInclude(x => x.Attributes)
                .Include(x => x.Attributes)
                .FirstOrDefaultAsync(e => e.Id == id) ?? throw new Exception("实体信息未找到");

            db.EntityInfos.Remove(entityInfo);
            await db.SaveChangesAsync();
            return AmisResult.Ok();
        }

        [HttpPost("{id}")]
        public async Task<AmisResult<EntityInfo>> UpdateFromPathAsync(long id)
        {
            var entityInfo = await db.EntityInfos
                .FirstOrDefaultAsync(e => e.Id == id) ?? throw new Exception("实体信息未找到");

            await CreateOrUpdateByFileAsync(entityInfo.FilePath, new List<EntityInfo>() { entityInfo });

            await db.SaveChangesAsync();
            return AmisResult.Ok(entityInfo);
        }

        [HttpPut("{id}")]
        public async Task<AmisResult<EntityInfo>> UpdateAsync(long id, EntityInfo info)
        {
            var entityInfo = await db.EntityInfos
                .FirstOrDefaultAsync(e => e.Id == id) ?? throw new Exception("实体信息未找到");

            db.Entry(entityInfo).CurrentValues.SetValues(info);

            await db.SaveChangesAsync();
            return AmisResult.Ok(entityInfo);
        }

        #region 生成代码


        [HttpPost("{entityInfoId}/Preview")]
        public async Task<AmisResult<List<GeneratorOutput>>> PreviewAsync(long entityInfoId, [FromQuery] string? templateIds)
        {
            var templateIdArr = templateIds?.Split(",").Select(x => long.Parse(x)).ToArray() ?? [];

            if (templateIdArr.Length == 0) return AmisResult.Ok(new List<GeneratorOutput>());

            var templates = await db.Templates.OrderBy(x => x.Name).Where(x => templateIdArr.Contains(x.Id)).ToListAsync();
            var entityInfo = await db.EntityInfos
                .Include(x => x.Attributes)
                .Include(x => x.Properties).ThenInclude(x => x.Attributes)
                .FirstOrDefaultAsync(x => x.Id == entityInfoId) ?? throw new Exception("实体信息未找到");
            var configuration = await db.Configurations.FirstOrDefaultAsync() ?? new();

            var list = new List<GeneratorOutput>();
            foreach (var template in templates)
            {
                var (code, outputPath) = CodeGenerator(template, entityInfo, configuration);
                var generatorOutput = new GeneratorOutput(template.Name, code, outputPath, template.Overwrite);
                list.Add(generatorOutput);
            }

            return AmisResult.Ok(list);
        }

        private (string, string) CodeGenerator(Template template, EntityInfo entityInfo, Configuration configuration)
        {
            // 根据实体信息的OutputPath解析输出路径
            string outputPath = template.OutputPath
                .Replace("${SlnDir}", configuration.SlnDir ?? "")
                .Replace("${EntityFolder}", entityInfo.Folder ?? "Entities")
                .Replace("${EntityName}", entityInfo.ClassName)
                .Replace("${entityName}", entityInfo.ClassNameJs)
                ;

            entityInfo.Properties = entityInfo.Properties.Where(x => (entityInfo.IgnoreProperties ?? "").Contains(x.Name) == false).ToList();

            // 根据Razor模板内容生成代码
            var code = Engine.Razor.RunCompile(template.Content, template.LastModified.Ticks.ToString(), entityInfo.GetType(), entityInfo);
            //string code = RazorEngine.Razor.Parse(template.Content, entityInfo);

            return (code, outputPath);
        }

        [HttpPost("{entityInfoId}/TemplateGenerator/{templateIds}")]
        public async Task<AmisResult> TemplateGeneratorAsync(long entityInfoId, string templateIds)
        {
            var res = await PreviewAsync(entityInfoId, templateIds);
            foreach (var item in res.Data)
            {
                WriteToFile(item);
            }

            return AmisResult.Ok();
        }

        [HttpPost("SolutionsGenerator")]
        public async Task<AmisResult> SolutionsGeneratorAsync([FromBody] SolutionsGeneratorInput input)
        {
            if (input.SolutionsId == 0) { return AmisResult.Fail("请选择解决方案"); }
            if (string.IsNullOrEmpty(input.Ids)) { /*throw new Exception("请选择实体");*/ return AmisResult.Fail("请选择实体"); }

            var solutions = await db.Solutionss.Include(s => s.Templates).Where(x => x.Id == input.SolutionsId).FirstOrDefaultAsync();
            var templateIds = string.Join(",", solutions!.Templates.Select(x => x.Id));

            foreach (var entityInfoIdText in input.Ids.Split(","))
            {
                long.TryParse(entityInfoIdText, out long entityInfoId);
                var res = await PreviewAsync(entityInfoId, templateIds);
                foreach (var item in res.Data)
                {
                    WriteToFile(item);
                }
            }

            return AmisResult.Ok();
        }

        private void WriteToFile(GeneratorOutput item)
        {
            //判断文件名是否为空
            if (string.IsNullOrWhiteSpace(item.OutputPath))
            {
                throw new Exception("输出路径为空");
            }

            //根据文件名获取文件夹路径，判断文件夹是否存在
            var folderPath = Path.GetDirectoryName(item.OutputPath);
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                throw new Exception("输出路径文件夹为空");
            }

            if (!Directory.Exists(folderPath))
            {
                //如果不存在，则创建文件夹
                Directory.CreateDirectory(folderPath);
            }

            // 判断Code是否为空
            if (string.IsNullOrWhiteSpace(item.Code))
            {
                throw new Exception("代码为空");
            }

            // 如果不覆盖，并且文件存在，则直接返回，不进行覆盖操作
            if (!item.Overwrite && System.IO.File.Exists(item.OutputPath))
            {
                return;
            }

            System.IO.File.WriteAllText(item.OutputPath, item.Code);
        }


        #endregion
    }
}
