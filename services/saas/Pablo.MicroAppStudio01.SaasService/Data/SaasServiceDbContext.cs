using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;
using Volo.Saas.Editions;
using Volo.Saas.EntityFrameworkCore;
using Volo.Saas.Tenants;

namespace Pablo.MicroAppStudio01.SaasService.Data;

[ConnectionStringName(DatabaseName)]
[ReplaceDbContext(typeof(ISaasDbContext))]
public class SaasServiceDbContext :
    AbpDbContext<SaasServiceDbContext>,
    ISaasDbContext,
    IHasEventInbox,
    IHasEventOutbox
{
    public const string DbTablePrefix = "";
    public const string DbSchema = null;

    public const string DatabaseName = "SaasService";
    
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Edition> Editions { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }
    
    public DbSet<IncomingEventRecord> IncomingEvents { get; set; }
    public DbSet<OutgoingEventRecord> OutgoingEvents { get; set; }

    public SaasServiceDbContext(DbContextOptions<SaasServiceDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ConfigureEventInbox();
        builder.ConfigureEventOutbox();
        builder.ConfigureSaas();
    }
}