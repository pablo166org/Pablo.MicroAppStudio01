using Volo.Abp.EntityFrameworkCore.Migrations;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.DistributedLocking;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace Pablo.MicroAppStudio01.IdentityService.Data;

public class IdentityServiceDatabaseMigrationEventHandler : EfCoreDatabaseMigrationEventHandlerBase<IdentityServiceDbContext>
{
    private readonly IdentityServiceDataSeeder _dataSeeder;

    public IdentityServiceDatabaseMigrationEventHandler(
        ILoggerFactory loggerFactory,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager,
        ITenantStore tenantStore,
        IAbpDistributedLock abpDistributedLock,
        IDistributedEventBus distributedEventBus,
        IdentityServiceDataSeeder dataSeeder
    ) : base(
        IdentityServiceDbContext.DatabaseName,
        currentTenant,
        unitOfWorkManager,
        tenantStore,
        abpDistributedLock,
        distributedEventBus,
        loggerFactory)
    {
        _dataSeeder = dataSeeder;
    }

    protected override async Task SeedAsync(Guid? tenantId)
    {
        await _dataSeeder.SeedAsync(tenantId);
    }

    public override async Task HandleEventAsync(TenantCreatedEto eventData)
    {
        try
        {
            await MigrateDatabaseSchemaAsync(eventData.Id);
            await _dataSeeder.SeedAsync(
                tenantId: eventData.Id,
                adminEmail: eventData.Properties.GetOrDefault(IdentityDataSeedContributor.AdminEmailPropertyName),
                adminPassword: eventData.Properties.GetOrDefault(IdentityDataSeedContributor.AdminPasswordPropertyName) 
            );
        }
        catch (Exception ex)
        {
            await HandleErrorTenantCreatedAsync(eventData, ex);
        }
    }
}