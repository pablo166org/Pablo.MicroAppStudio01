{{~
    if config.database_provider != "ef"
    helpers.delete_this_file
    else
    helpers.rename_this_file("MicroserviceNameDataSeeder.cs")
    end
~}}
using Volo.Abp.DependencyInjection;

namespace Pablo.MicroAppStudio01.MicroserviceName.Data;

public class MicroserviceNameDataSeeder : ITransientDependency
{
    private readonly ILogger<MicroserviceNameDataSeeder> _logger;

    public MicroserviceNameDataSeeder(
        ILogger<MicroserviceNameDataSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(Guid? tenantId = null)
    {
        _logger.LogInformation("Seeding data...");
        
        //...
    }
}