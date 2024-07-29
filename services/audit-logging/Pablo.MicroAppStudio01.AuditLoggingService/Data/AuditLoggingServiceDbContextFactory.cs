using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pablo.MicroAppStudio01.AuditLoggingService.Data;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands)
 * */
public class AuditLoggingServiceDbContextFactory : IDesignTimeDbContextFactory<AuditLoggingServiceDbContext>
{
    public AuditLoggingServiceDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<AuditLoggingServiceDbContext>()
        .UseSqlServer(GetConnectionStringFromConfiguration(), b =>
        {
            b.MigrationsHistoryTable("__AuditLoggingService_Migrations");
        });

        return new AuditLoggingServiceDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(AuditLoggingServiceDbContext.DatabaseName)
               ?? throw new ApplicationException($"Could not find a connection string named '{AuditLoggingServiceDbContext.DatabaseName}'.");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
