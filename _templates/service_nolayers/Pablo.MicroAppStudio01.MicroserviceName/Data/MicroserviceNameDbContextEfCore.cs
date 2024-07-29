{{~
    if config.database_provider != "ef"
    helpers.delete_this_file
    else
    helpers.rename_this_file("MicroserviceNameDbContext.cs")
    end
~}}
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;

namespace Pablo.MicroAppStudio01.MicroserviceName.Data;

[ConnectionStringName(DatabaseName)]
public class MicroserviceNameDbContext :
    AbpDbContext<MicroserviceNameDbContext>,
    IHasEventInbox,
    IHasEventOutbox
{
    public const string DbTablePrefix = "";
    public const string DbSchema = null;
    
    public const string DatabaseName = "MicroserviceName";
    
    public DbSet<IncomingEventRecord> IncomingEvents { get; set; }
    public DbSet<OutgoingEventRecord> OutgoingEvents { get; set; }

    public MicroserviceNameDbContext(DbContextOptions<MicroserviceNameDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureEventInbox();
        builder.ConfigureEventOutbox();
    }
}