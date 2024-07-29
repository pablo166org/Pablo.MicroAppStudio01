{{~
    if config.database_provider != "ef"
    helpers.delete_this_file
    end
~}}
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pablo.MicroAppStudio01.MicroserviceName.Data;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands)
 * */
public class MicroserviceNameDbContextFactory : IDesignTimeDbContextFactory<MicroserviceNameDbContext>
{
    public MicroserviceNameDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<MicroserviceNameDbContext>()
            .UseSqlServer(GetConnectionStringFromConfiguration(), b =>
            {
                b.MigrationsHistoryTable("__MicroserviceName_Migrations");
            });

        return new MicroserviceNameDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(MicroserviceNameDbContext.DatabaseName)
               ?? throw new ApplicationException($"Could not find a connection string named '{MicroserviceNameDbContext.DatabaseName}'.");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
