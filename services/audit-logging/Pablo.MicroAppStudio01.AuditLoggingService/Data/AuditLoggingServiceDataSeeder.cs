using Volo.Abp.DependencyInjection;

namespace Pablo.MicroAppStudio01.AuditLoggingService.Data;

public class AuditLoggingServiceDataSeeder : ITransientDependency
{
    private readonly ILogger<AuditLoggingServiceDataSeeder> _logger;

    public AuditLoggingServiceDataSeeder(
        ILogger<AuditLoggingServiceDataSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(Guid? tenantId = null)
    {
        _logger.LogInformation("Seeding data...");
        
        //...
    }
}