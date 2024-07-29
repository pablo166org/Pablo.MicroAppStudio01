using Volo.Abp.BlobStoring.Database;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EntityFrameworkCore.Migrations;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Pablo.MicroAppStudio01.AdministrationService.Data;

public class BlobStoringRuntimeDatabaseMigrator : EfCoreRuntimeDatabaseMigratorBase<BlobStoringDbContext>
{
    public BlobStoringRuntimeDatabaseMigrator(
        IUnitOfWorkManager unitOfWorkManager,
        IServiceProvider serviceProvider,
        ICurrentTenant currentTenant,
        IAbpDistributedLock abpDistributedLock,
        IDistributedEventBus distributedEventBus,
        ILoggerFactory loggerFactory
    ) : base(
        AbpBlobStoringDatabaseDbProperties.ConnectionStringName,
        unitOfWorkManager,
        serviceProvider,
        currentTenant,
        abpDistributedLock,
        distributedEventBus,
        loggerFactory)
    {
    }
}