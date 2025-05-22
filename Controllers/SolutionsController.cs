using CodeFirstGenerator.Data;
using CodeFirstGenerator.Dtos;
using CodeFirstGenerator.Entities;
using CodeFirstGenerator.Migrations;
using ELF.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstGenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolutionsController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public SolutionsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        public async Task<AmisResult<Solutions>> CreateAsync(SolutionsInput input)
        {
            var solutions = new Solutions();
            db.Solutionss.Add(solutions);
            db.Entry(solutions).CurrentValues.SetValues(input);
            await db.SaveChangesAsync();
            return AmisResult.Ok(solutions);
        }

        [HttpGet("{id}")]
        public async Task<AmisResult<Solutions>> GetAsync(long id)
        {
            var solutions = await db.Solutionss
                .Include(x => x.Templates)
                .FirstOrDefaultAsync(e => e.Id == id) ?? throw new Exception("方案未找到");
            return AmisResult.Ok(solutions);
        }

        [HttpGet]
        public async Task<AmisResult<AmisPagedResultDto<Solutions>>> GetListAsync([FromQuery] SolutionsGetListInput input)
        {
            var query = db.Solutionss.AsNoTracking()
                .WhereNotNull(input.Name, x => x.Name.Contains(input.Name!))
                .WhereNotNull(input.Description, x => x.Description!.Contains(input.Description!))
                ;
            var totalCount = await query.CountAsync();

            var entities = new List<Solutions>();

            if (totalCount > 0)
            {
                input.OrderBy = input.OrderBy ?? nameof(input.Name);
                query = query.AmisOrderBy(input);
                query = query.AmisPaging(input);

                entities = await query.ToListAsync();
            }

            return AmisResult.Ok(new AmisPagedResultDto<Solutions>(
                totalCount,
                entities
            ));
        }

        [HttpDelete("{id}")]
        public async Task<AmisResult> DeleteAsync(long id)
        {
            var solutions = await db.Solutionss
                .FirstOrDefaultAsync(e => e.Id == id) ?? throw new Exception("方案未找到");

            db.Solutionss.Remove(solutions);
            await db.SaveChangesAsync();
            return AmisResult.Ok();
        }

        [HttpPut("{id}")]
        public async Task<AmisResult<Solutions>> UpdateAsync(long id, SolutionsInput input)
        {
            var solutions = await GetAsync(id);

            //for循环，倒叙，Remove所有solution.Templates
            for (int i = solutions.Data.Templates.Count - 1; i >= 0; i--)
            {
                solutions.Data.Templates.Remove(solutions.Data.Templates[i]);
            }

            db.Entry(solutions.Data).CurrentValues.SetValues(input);

            if (!string.IsNullOrEmpty(input.TemplateIds))
            {
                var templateIds = input.TemplateIds.Split(',').Select(x => long.Parse(x)).ToArray();
                var templates = await db.Templates.Where(x => templateIds.Contains(x.Id)).ToListAsync();
                solutions.Data.Templates.AddRange(templates);
            }

            await db.SaveChangesAsync();
            return solutions;
        }

        [HttpGet("{id}/TemplateIds")]
        public async Task<AmisResult<string>> TemplateIdsAsync(long id)
        {
            var solutions = await db.Solutionss
                .Include(x => x.Templates)
                .FirstOrDefaultAsync(e => e.Id == id) ?? throw new Exception("方案未找到");

            return AmisResult.Ok(solutions.TemplateIds);
        }

    }
}
