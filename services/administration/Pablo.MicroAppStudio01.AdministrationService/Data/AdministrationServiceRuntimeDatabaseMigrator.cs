using Volo.Abp.DistributedLocking;
using Volo.Abp.EntityFrameworkCore.Migrations;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Pablo.MicroAppStudio01.AdministrationService.Data;

public class AdministrationServiceRuntimeDatabaseMigrator : EfCoreRuntimeDatabaseMigratorBase<AdministrationServiceDbContext>
{
    private readonly AdministrationServiceDataSeeder _administrationServiceDataSeeder;

    public AdministrationServiceRuntimeDatabaseMigrator(
        ILoggerFactory loggerFactory,
        IUnitOfWorkManager unitOfWorkManager,
        IServiceProvider serviceProvider,
        ICurrentTenant currentTenant,
        IAbpDistributedLock abpDistributedLock,
        IDistributedEventBus distributedEventBus,
        AdministrationServiceDataSeeder administrationServiceDataSeeder
    ) : base(
        AdministrationServiceDbContext.DatabaseName,
        unitOfWorkManager,
        serviceProvider,
        currentTenant,
        abpDistributedLock,
        distributedEventBus,
        loggerFactory)
    {
        _administrationServiceDataSeeder = administrationServiceDataSeeder;
    }

    protected override async Task SeedAsync()
    {
        await _administrationServiceDataSeeder.SeedAsync();
    }
}