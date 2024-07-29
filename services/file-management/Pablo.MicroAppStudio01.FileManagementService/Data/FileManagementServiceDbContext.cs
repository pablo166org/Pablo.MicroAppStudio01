using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;
using Volo.FileManagement.EntityFrameworkCore;
using Volo.FileManagement.Directories;
using Volo.FileManagement.Files;

namespace Pablo.MicroAppStudio01.FileManagementService.Data;

[ConnectionStringName(DatabaseName)]
[ReplaceDbContext(typeof(IFileManagementDbContext))]
public class FileManagementServiceDbContext :
    AbpDbContext<FileManagementServiceDbContext>,
    IFileManagementDbContext,
    IHasEventInbox,
    IHasEventOutbox
{
    public const string DbTablePrefix = "";
    public const string DbSchema = null;
    
    public const string DatabaseName = "FileManagementService";
    
    public DbSet<IncomingEventRecord> IncomingEvents { get; set; }
    public DbSet<OutgoingEventRecord> OutgoingEvents { get; set; }

    public DbSet<DirectoryDescriptor> DirectoryDescriptions { get; set; }
    public DbSet<FileDescriptor> FileDescriptions { get; set; }

    public FileManagementServiceDbContext(DbContextOptions<FileManagementServiceDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureEventInbox();
        builder.ConfigureEventOutbox();
        builder.ConfigureFileManagement();
    }
}