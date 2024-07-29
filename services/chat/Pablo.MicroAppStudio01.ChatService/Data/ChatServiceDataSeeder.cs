using Volo.Abp.DependencyInjection;

namespace Pablo.MicroAppStudio01.ChatService.Data;

public class ChatServiceDataSeeder : ITransientDependency
{
    private readonly ILogger<ChatServiceDataSeeder> _logger;

    public ChatServiceDataSeeder(
        ILogger<ChatServiceDataSeeder> logger)
    {
        _logger = logger;
    }

    public async Task SeedAsync(Guid? tenantId = null)
    {
        _logger.LogInformation("Seeding data...");
        
        //...
    }
}