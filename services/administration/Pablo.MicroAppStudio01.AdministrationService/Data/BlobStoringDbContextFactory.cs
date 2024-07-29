using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Volo.Abp.BlobStoring.Database;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;

namespace Pablo.MicroAppStudio01.AdministrationService.Data;

/* This class is needed for EF Core console commands
 * To Add Add New Migration, execute the following command a command-line terminal in this project's root folder:
 * 
 *   dotnet ef migrations add "My_Migration_Name" -c BlobStoringDbContext -o Migrations/BlobStoring
 * 
 * */
public class BlobStoringDbContextFactory : IDesignTimeDbContextFactory<BlobStoringDbContext>
{
    public BlobStoringDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<BlobStoringDbContext>()
            .UseSqlServer(GetConnectionStringFromConfiguration(), b =>
            {
                b.MigrationsHistoryTable("__AbpBlobStoring_Migrations");
                b.MigrationsAssembly(typeof(MicroAppStudio01AdministrationServiceModule).Assembly.GetName().Name);
            });

        return new BlobStoringDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(AbpBlobStoringDatabaseDbProperties.ConnectionStringName) 
               ?? throw new ApplicationException($"Could not find a connection string named '{AbpBlobStoringDatabaseDbProperties.ConnectionStringName}'.");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}