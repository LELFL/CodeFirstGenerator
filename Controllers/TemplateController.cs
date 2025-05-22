using CodeFirstGenerator.Data;
using CodeFirstGenerator.Dtos;
using CodeFirstGenerator.Entities;
using ELF.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Yitter.IdGenerator;

namespace CodeFirstGenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemplateController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public TemplateController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public async Task<AmisResult<Template>> CreateAsync(TemplateInput input)
        {
            var template = new Template();
            db.Templates.Add(template);
            db.Entry(template).CurrentValues.SetValues(input);
            await db.SaveChangesAsync();
            return AmisResult.Ok(template);
        }

        [HttpGet("{id}")]
        public async Task<AmisResult<Template>> GetAsync(long id)
        {
            var entityInfo = await db.Templates
                .FirstOrDefaultAsync(e => e.Id == id) ?? throw new Exception("模板未找到");
            return AmisResult.Ok(entityInfo);
        }

        [HttpGet]
        public async Task<AmisResult<AmisPagedResultDto<Template>>> GetListAsync([FromQuery] TemplateGetListInput input)
        {
            var query = db.Templates.AsNoTracking()
                .WhereNotNull(input.Name, x => x.Name.Contains(input.Name!))
                .WhereNotNull(input.TemplateType, x => input.TemplateType!.Contains(x.TemplateType))
                .WhereNotNull(input.Content, x => x.Content.Contains(input.Content!))
                .WhereNotNull(input.OutputPath, x => x.OutputPath.Contains(input.OutputPath!))
                .WhereNotNull(input.Overwrite, x => x.Overwrite == input.Overwrite)
                ;
            var totalCount = await query.CountAsync();

            var entities = new List<Template>();

            if (totalCount > 0)
            {
                input.OrderBy = input.OrderBy ?? nameof(input.Name);
                query = query.AmisOrderBy(input);
                query = query.AmisPaging(input);

                entities = await query.ToListAsync();
            }

            return AmisResult.Ok(new AmisPagedResultDto<Template>(
                totalCount,
                entities
            ));
        }

        [HttpDelete("{id}")]
        public async Task<AmisResult> DeleteAsync(long id)
        {
            var entityInfo = await db.Templates
                .FirstOrDefaultAsync(e => e.Id == id) ?? throw new Exception("模板未找到");

            db.Templates.Remove(entityInfo);
            await db.SaveChangesAsync();
            return AmisResult.Ok();
        }

        [HttpPut("{id}")]
        public async Task<AmisResult<Template>> UpdateAsync(long id, TemplateInput input)
        {
            var template = await GetAsync(id);
            db.Entry(template.Data).CurrentValues.SetValues(input);
            template.Data.RazorId = YitIdHelper.NextId();
            await db.SaveChangesAsync();
            return template;
        }
    }
}
