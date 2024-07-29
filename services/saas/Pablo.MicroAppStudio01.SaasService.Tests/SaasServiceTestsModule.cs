using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pablo.MicroAppStudio01.SaasService.Data;
using Volo.Abp;
using Volo.Abp.AspNetCore.TestBase;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.DistributedLocking;
using Volo.Abp.EventBus;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Uow;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.MultiTenancy;
using Volo.Abp.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Saas.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;

namespace Pablo.MicroAppStudio01.SaasService.Tests;

/* This project has a dotnet project reference to the Pablo.MicroAppStudio01.SaasService project,
 * but it does not have a module dependency to the MicroAppStudio01SaasServiceModule module class.
 * Because, MicroAppStudio01SaasServiceModule has configurations proper for development and production
 * environments, but not proper or necessary for tests.
 *
 * In this test project, we are carefully depending on the modules that we need in tests.
 *
 * For example, MicroAppStudio01SaasServiceModule depends on AbpEventBusRabbitMqModule,
 * but this module depends on AbpEventBusModule since we don't want to use RabbitMQ in tests.
 * AbpEventBusModule has an in-process event bus instead of a real distributed event bus, and it is fine for tests.
 *
 * WARNING: If you change MicroAppStudio01SaasServiceModule class, you may need to properly change this class to keep
 *          test code compatible with the application code.
 */
[DependsOn(
    typeof(AbpAspNetCoreTestBaseModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(SaasEntityFrameworkCoreModule),
    typeof(AbpEventBusModule),
    typeof(AbpCachingModule),
    typeof(AbpDistributedLockingAbstractionsModule)
)]
[AdditionalAssembly(typeof(MicroAppStudio01SaasServiceModule))]
public class SaasServiceTestsModule : AbpModule
{
    private SqliteConnection? _sqliteConnection;
    
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        
        ConfigureAuthorization(context);
        ConfigureMultiTenancy();
        ConfigureDatabase(context);
        ConfigureDatabaseTransactions(context);
        ConfigureDynamicStores();
        ConfigureBackgroundJobs();
    }

    private void ConfigureMultiTenancy()
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = true;
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        app.UseCorrelationId();
        app.UseAbpRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseMultiTenancy();
        app.UseUnitOfWork();
        app.UseConfiguredEndpoints();
    }

    public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        using var scope = context.ServiceProvider.CreateScope();
        await scope.ServiceProvider
            .GetRequiredService<SaasServiceDataSeeder>()
            .SeedAsync();
    }
    
    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        _sqliteConnection?.Dispose();
    }

    private static void ConfigureAuthorization(ServiceConfigurationContext context)
    {
        /* We don't need to authorization in tests */
        context.Services.AddAlwaysAllowAuthorization();
    }

    private void ConfigureDatabase(ServiceConfigurationContext context)
    {
        _sqliteConnection = CreateDatabaseAndGetConnection();
        
        context.Services.AddAbpDbContext<SaasServiceDbContext>(options =>
        {
            options.AddDefaultRepositories();
        });
        
        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(opts =>
            {
                /* Use SQLite for all EF Core DbContexts in tests */
                opts.UseSqlite(_sqliteConnection);
            });
        });
    }
    
    private void ConfigureDatabaseTransactions(ServiceConfigurationContext context)
    {
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();
        
        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });
    }
    
    private void ConfigureDynamicStores()
    {
        Configure<FeatureManagementOptions>(options =>
        {
            options.SaveStaticFeaturesToDatabase = false;
            options.IsDynamicFeatureStoreEnabled = false;
        });
        
        Configure<PermissionManagementOptions>(options =>
        {
            options.SaveStaticPermissionsToDatabase = false;
            options.IsDynamicPermissionStoreEnabled = false;
        });

        Configure<SettingManagementOptions>(options =>
        {
            options.SaveStaticSettingsToDatabase = false;
            options.IsDynamicSettingStoreEnabled = false;
        });
        
        //TODO: text templates
    }
    
    private void ConfigureBackgroundJobs()
    {
        Configure<AbpBackgroundWorkerOptions>(options =>
        {
            options.IsEnabled = false;
        });
        
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });
    }
    
    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        // SaasServiceDbContext ()
        new SaasServiceDbContext(
            new DbContextOptionsBuilder<SaasServiceDbContext>().UseSqlite(connection).Options
        ).GetService<IRelationalDatabaseCreator>().CreateTables();
        
        // PermissionManagementDbContext
        new PermissionManagementDbContext(
            new DbContextOptionsBuilder<PermissionManagementDbContext>().UseSqlite(connection).Options
        ).GetService<IRelationalDatabaseCreator>().CreateTables();
        
        // FeatureManagementDbContext
        new FeatureManagementDbContext(
            new DbContextOptionsBuilder<FeatureManagementDbContext>().UseSqlite(connection).Options
        ).GetService<IRelationalDatabaseCreator>().CreateTables();
        
        // SettingManagementDbContext
        new SettingManagementDbContext(
            new DbContextOptionsBuilder<SettingManagementDbContext>().UseSqlite(connection).Options
        ).GetService<IRelationalDatabaseCreator>().CreateTables();
        
        // BlobStoringDbContext
        new BlobStoringDbContext(
            new DbContextOptionsBuilder<BlobStoringDbContext>().UseSqlite(connection).Options
        ).GetService<IRelationalDatabaseCreator>().CreateTables();

        return connection;
    }
}
