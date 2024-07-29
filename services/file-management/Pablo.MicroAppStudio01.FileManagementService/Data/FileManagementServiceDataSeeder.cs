using Volo.Abp.DependencyInjection;

namespace Pablo.MicroAppStudio01.FileManagementService.Data;

public class FileManagementServiceDataSeeder : ITransientDependency
{
    private readonly ILogger<FileManagementServiceDataSeeder> _logger;

    public FileManagementServiceDataSeeder(
        ILogger<FileManagementServiceDataSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(Guid? tenantId = null)
    {
        _logger.LogInformation("Seeding data...");
        
        //...
    }
}