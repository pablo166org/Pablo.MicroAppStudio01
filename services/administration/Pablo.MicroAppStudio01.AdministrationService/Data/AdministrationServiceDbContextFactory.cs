using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pablo.MicroAppStudio01.AdministrationService.Data;

/* This class is needed for EF Core console commands
 * To Add Add New Migration, execute the following command a command-line terminal in this project's root folder:
 * 
 *   dotnet ef migrations add "My_Migration_Name" -c AdministrationServiceDbContext -o Migrations
 * 
 * */
public class AdministrationServiceDbContextFactory : IDesignTimeDbContextFactory<AdministrationServiceDbContext>
{
    public AdministrationServiceDbContext CreateDbContext(string[] args)
    {
        AdministrationServiceEfCoreEntityExtensionMappings.Configure();

        var builder = new DbContextOptionsBuilder<AdministrationServiceDbContext>()
        .UseSqlServer(GetConnectionStringFromConfiguration(), b =>
        {
            b.MigrationsHistoryTable("__AdministrationService_Migrations");
        });

        return new AdministrationServiceDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(AdministrationServiceDbContext.DatabaseName) 
               ?? throw new ApplicationException($"Could not find a connection string named '{AdministrationServiceDbContext.DatabaseName}'.");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands)
 * */