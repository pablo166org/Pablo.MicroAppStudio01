using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pablo.MicroAppStudio01.SaasService.Data;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands)
 * */
public class SaasServiceDbContextFactory : IDesignTimeDbContextFactory<SaasServiceDbContext>
{
    public SaasServiceDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<SaasServiceDbContext>()
        .UseSqlServer(GetConnectionStringFromConfiguration(), b =>
        {
            b.MigrationsHistoryTable("__SaasService_Migrations");
        });

        return new SaasServiceDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(SaasServiceDbContext.DatabaseName)
               ?? throw new ApplicationException($"Could not find a connection string named '{SaasServiceDbContext.DatabaseName}'.");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
