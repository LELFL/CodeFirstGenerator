using CodeFirstGenerator.Data;
using CodeFirstGenerator.Entities;
using ELF.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace CodeFirstGenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly ApplicationDbContext db;

        public ConfigurationController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<AmisResult<Configuration>> GetAsync()
        {
            var entityInfo = await db.Configurations.FirstOrDefaultAsync();
            if (entityInfo == null)
            {
                entityInfo = new Configuration();
                db.Configurations.Add(entityInfo);
                //await db.SaveChangesAsync();
            }
            return AmisResult.Ok(entityInfo);
        }

        [HttpPut]
        public async Task<AmisResult<Configuration>> UpdateAsync(Configuration input)
        {
            var configuration = await GetAsync();
            db.Entry(configuration.Data).CurrentValues.SetValues(input);
            await db.SaveChangesAsync();
            return configuration;
        }
    }
}
