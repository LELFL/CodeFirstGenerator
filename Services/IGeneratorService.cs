using CodeFirstGenerator.Entities;

namespace CodeFirstGenerator.Controllers
{
    public interface IGeneratorService
    {

        Task<List<EntityInfo>> ParseClassCodeAsync(string code);

    }
}
