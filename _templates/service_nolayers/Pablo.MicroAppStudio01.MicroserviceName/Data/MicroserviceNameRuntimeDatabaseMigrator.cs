{{~
    if config.database_provider != "ef"
    helpers.delete_this_file
    end
~}}
using Volo.Abp.DistributedLocking;
using Volo.Abp.EntityFrameworkCore.Migrations;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Pablo.MicroAppStudio01.MicroserviceName.Data;

public class MicroserviceNameRuntimeDatabaseMigrator : EfCoreRuntimeDatabaseMigratorBase<MicroserviceNameDbContext>
{
    private readonly MicroserviceNameDataSeeder _dataSeeder;

    public MicroserviceNameRuntimeDatabaseMigrator(
        ILoggerFactory loggerFactory,
        IUnitOfWorkManager unitOfWorkManager,
        IServiceProvider serviceProvider,
        ICurrentTenant currentTenant,
        IAbpDistributedLock abpDistributedLock,
        IDistributedEventBus distributedEventBus,
        MicroserviceNameDataSeeder dataSeeder
    ) : base(
        MicroserviceNameDbContext.DatabaseName,
        unitOfWorkManager,
        serviceProvider,
        currentTenant,
        abpDistributedLock,
        distributedEventBus,
        loggerFactory)
    {
        _dataSeeder = dataSeeder;
    }

    protected override async Task SeedAsync()
    {
        await _dataSeeder.SeedAsync();
    }
}