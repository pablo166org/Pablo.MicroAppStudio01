using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pablo.MicroAppStudio01.ChatService.Data;

/* This class is needed for EF Core console commands
 * (like Add-Migration and Update-Database commands)
 * */
public class ChatServiceDbContextFactory : IDesignTimeDbContextFactory<ChatServiceDbContext>
{
    public ChatServiceDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<ChatServiceDbContext>()
        .UseSqlServer(GetConnectionStringFromConfiguration(), b =>
        {
            b.MigrationsHistoryTable("__ChatService_Migrations");
        });

        return new ChatServiceDbContext(builder.Options);
    }
    
    private static string GetConnectionStringFromConfiguration()
    {
        return BuildConfiguration().GetConnectionString(ChatServiceDbContext.DatabaseName)
               ?? throw new ApplicationException($"Could not find a connection string named '{ChatServiceDbContext.DatabaseName}'.");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
