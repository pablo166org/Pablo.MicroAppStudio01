using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pablo.MicroAppStudio01.FileManagementService.Data;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands)
 * */
public class FileManagementServiceDbContextFactory : IDesignTimeDbContextFactory<FileManagementServiceDbContext>
{
    public FileManagementServiceDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<FileManagementServiceDbContext>()
        .UseSqlServer(GetConnectionStringFromConfiguration(), b =>
        {
            b.MigrationsHistoryTable("__FileManagementService_Migrations");
        });

        return new FileManagementServiceDbContext(builder.Options);
    }

    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(FileManagementServiceDbContext.DatabaseName)
               ?? throw new ApplicationException($"Could not find a connection string named '{FileManagementServiceDbContext.DatabaseName}'.");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
