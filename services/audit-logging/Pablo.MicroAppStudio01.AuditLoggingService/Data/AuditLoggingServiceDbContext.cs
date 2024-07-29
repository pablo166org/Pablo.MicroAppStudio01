using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;

namespace Pablo.MicroAppStudio01.AuditLoggingService.Data;

[ConnectionStringName(DatabaseName)]
public class AuditLoggingServiceDbContext :
    AbpDbContext<AuditLoggingServiceDbContext>,
    IAuditLoggingDbContext,
    IHasEventInbox,
    IHasEventOutbox
{
    public const string DbTablePrefix = "";
    public const string DbSchema = null;

    public const string DatabaseName = "AuditLoggingService";
    
    public DbSet<IncomingEventRecord> IncomingEvents { get; set; }
    public DbSet<OutgoingEventRecord> OutgoingEvents { get; set; }

    public DbSet<AuditLog> AuditLogs { get; set; }
    public AuditLoggingServiceDbContext(DbContextOptions<AuditLoggingServiceDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureEventInbox();
        builder.ConfigureEventOutbox();
        builder.ConfigureAuditLogging();
    }
}