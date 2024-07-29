{{~
    if config.database_provider != "mongodb"
    helpers.delete_this_file
    else
    helpers.rename_this_file("MicroserviceNameDatabaseMigrationEventHandler.cs")
    end
~}}
using Volo.Abp.MongoDB.Migrations;

namespace Pablo.MicroAppStudio01.MicroserviceName.Data;

public class MicroserviceNameDatabaseMigrationEventHandler: MongoDatabaseMigrationEventHandlerBase<MicroserviceNameDbContext>
{
    private readonly MicroserviceNameDataSeeder _dataSeeder;

    public MicroserviceNameDatabaseMigrationEventHandler(
        MicroserviceNameDataSeeder dataSeeder
    ) : base(
        MicroserviceNameDbContext.DatabaseName)
    {
        _dataSeeder = dataSeeder;
    }

    protected override async Task SeedAsync(Guid? tenantId)
    {
        await _dataSeeder.SeedAsync(tenantId);
    }
}