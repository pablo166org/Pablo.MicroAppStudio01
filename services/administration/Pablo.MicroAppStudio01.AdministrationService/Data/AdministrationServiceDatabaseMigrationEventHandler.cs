using Volo.Abp.EntityFrameworkCore.Migrations;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.DistributedLocking;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Pablo.MicroAppStudio01.AdministrationService.Data;

public class AdministrationServiceDatabaseMigrationEventHandler : EfCoreDatabaseMigrationEventHandlerBase<AdministrationServiceDbContext>
{
    private readonly AdministrationServiceDataSeeder _dataSeeder;

    public AdministrationServiceDatabaseMigrationEventHandler(
        ILoggerFactory loggerFactory,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager,
        ITenantStore tenantStore,
        IAbpDistributedLock abpDistributedLock,
        IDistributedEventBus distributedEventBus,
        AdministrationServiceDataSeeder dataSeeder
    ) : base(
        AdministrationServiceDbContext.DatabaseName,
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
}