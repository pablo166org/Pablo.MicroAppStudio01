using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Pablo.MicroAppStudio01.MicroserviceName.Data;
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
{{~ if config.database_provider == "ef" ~}}
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
{{~ else if config.database_provider == "mongodb" ~}}
using Volo.Abp.Data;
using Volo.Abp.SettingManagement.MongoDB;
using Volo.Abp.PermissionManagement.MongoDB;
using Volo.Abp.FeatureManagement.MongoDB;
using Volo.Abp.BlobStoring.Database.MongoDB;
using Pablo.MicroAppStudio01.MicroserviceName.Tests.MongoDB;
{{~ end~}}

namespace Pablo.MicroAppStudio01.MicroserviceName.Tests;

/* This project has a dotnet project reference to the Pablo.MicroAppStudio01.MicroserviceName project,
 * but it does not have a module dependency to the MicroAppStudio01MicroserviceNameModule module class.
 * Because, MicroAppStudio01MicroserviceNameModule has configurations proper for development and production
 * environments, but not proper or necessary for tests.
 *
 * In this test project, we are carefully depending on the modules that we need in tests.
 *
 * For example, MicroAppStudio01MicroserviceNameModule depends on AbpEventBusRabbitMqModule,
 * but this module depends on AbpEventBusModule since we don't want to use RabbitMQ in tests.
 * AbpEventBusModule has an in-process event bus instead of a real distributed event bus, and it is fine for tests.
 *
 * WARNING: If you change MicroAppStudio01MicroserviceNameModule class, you may need to properly change this class to keep
 *          test code compatible with the application code.
 */
[DependsOn(
    typeof(AbpAspNetCoreTestBaseModule),
    {{~ if config.database_provider == "ef" ~}}
    typeof(AbpEntityFrameworkCoreSqliteModule),
    typeof(BlobStoringDatabaseEntityFrameworkCoreModule),
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    {{~ else if config.database_provider == "mongodb" ~}}
    typeof(BlobStoringDatabaseMongoDbModule),
    typeof(AbpSettingManagementMongoDbModule),
    typeof(AbpPermissionManagementMongoDbModule),
    typeof(AbpFeatureManagementMongoDbModule),
    {{~ end~}}
    typeof(AbpEventBusModule),
    typeof(AbpCachingModule),
    typeof(AbpDistributedLockingAbstractionsModule)
)]
[AdditionalAssembly(typeof(MicroAppStudio01MicroserviceNameModule))]
public class MicroserviceNameTestsModule : AbpModule
{
    {{~ if config.database_provider == "ef" ~}}
    private SqliteConnection? _sqliteConnection;
    {{~ end~}}
    
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        {{~ if config.database_provider == "mongodb" ~}}
        
        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MicroserviceNameMongoDbFixture.GetRandomConnectionString();
            options.ConnectionStrings["MicroserviceName"] = MicroserviceNameMongoDbFixture.GetRandomConnectionString();
            options.ConnectionStrings["Identity"] = MicroserviceNameMongoDbFixture.GetRandomConnectionString();
            options.ConnectionStrings["Administration"] = MicroserviceNameMongoDbFixture.GetRandomConnectionString();
            options.ConnectionStrings["AbpBlobStoring"] = MicroserviceNameMongoDbFixture.GetRandomConnectionString();
        });
        {{~ end~}}
        
        ConfigureAuthorization(context);
        ConfigureDatabase(context);
        ConfigureDatabaseTransactions(context);
        ConfigureDynamicStores();
        ConfigureBackgroundJobs();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        app.UseCorrelationId();
        app.UseAbpRequestLocalization();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseUnitOfWork();
        app.UseConfiguredEndpoints();
    }

    public override async Task OnPostApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        using var scope = context.ServiceProvider.CreateScope();
        await scope.ServiceProvider
            .GetRequiredService<MicroserviceNameDataSeeder>()
            .SeedAsync();
    }
    {{~ if config.database_provider == "ef" ~}}
    
    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        _sqliteConnection?.Dispose();
    }
    {{~ end~}}

    private static void ConfigureAuthorization(ServiceConfigurationContext context)
    {
        /* We don't need to authorization in tests */
        context.Services.AddAlwaysAllowAuthorization();
    }

    private void ConfigureDatabase(ServiceConfigurationContext context)
    {
        {{~ if config.database_provider == "ef" ~}}
        _sqliteConnection = CreateDatabaseAndGetConnection();
        
        context.Services.AddAbpDbContext<MicroserviceNameDbContext>(options =>
        {{~ else if config.database_provider == "mongodb" ~}}
        context.Services.AddMongoDbContext<MicroserviceNameDbContext>(options =>
        {{~ end~}}
        {
            options.AddDefaultRepositories();
        });
        {{~ if config.database_provider == "ef" ~}}
        
        Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(opts =>
            {
                /* Use SQLite for all EF Core DbContexts in tests */
                opts.UseSqlite(_sqliteConnection);
            });
        });
        {{~ end~}}
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
    {{~ if config.database_provider == "ef" ~}}
    
    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        // MicroserviceNameDbContext ()
        new MicroserviceNameDbContext(
            new DbContextOptionsBuilder<MicroserviceNameDbContext>().UseSqlite(connection).Options
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
    {{~ end~}}
}
