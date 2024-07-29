using Volo.Abp.DistributedLocking;
using Volo.Abp.EntityFrameworkCore.Migrations;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Pablo.MicroAppStudio01.IdentityService.Data;

public class IdentityServiceRuntimeDatabaseMigrator : EfCoreRuntimeDatabaseMigratorBase<IdentityServiceDbContext>
{
    private readonly IdentityServiceDataSeeder _identityServiceDataSeeder;

    public IdentityServiceRuntimeDatabaseMigrator(
        ILoggerFactory loggerFactory,
        IUnitOfWorkManager unitOfWorkManager,
        IServiceProvider serviceProvider,
        ICurrentTenant currentTenant,
        IAbpDistributedLock abpDistributedLock,
        IDistributedEventBus distributedEventBus,
        IdentityServiceDataSeeder identityServiceDataSeeder
    ) : base(
        IdentityServiceDbContext.DatabaseName,
        unitOfWorkManager,
        serviceProvider,
        currentTenant,
        abpDistributedLock,
        distributedEventBus,
        loggerFactory)
    {
        _identityServiceDataSeeder = identityServiceDataSeeder;
    }

    protected override async Task SeedAsync()
    {
        await _identityServiceDataSeeder.SeedAsync();
    }
}