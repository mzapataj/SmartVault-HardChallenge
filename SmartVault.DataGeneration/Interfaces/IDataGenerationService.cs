using SmartVault.DataGeneration.Dto;

namespace SmartVault.DataGeneration.Interfaces
{
    public interface IDataGenerationService
    {
        DataGenerationResult SeedDatabase();
    }
}