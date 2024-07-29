using Microsoft.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.DistributedEvents;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LanguageManagement.EntityFrameworkCore;
using Volo.Abp.LanguageManagement.External;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TextTemplateManagement.EntityFrameworkCore;
using Volo.Abp.TextTemplateManagement.TextTemplates;

namespace Pablo.MicroAppStudio01.AdministrationService.Data;

[ConnectionStringName(DatabaseName)]
[ReplaceDbContext(
    typeof(IPermissionManagementDbContext),
    typeof(IFeatureManagementDbContext),
    typeof(ISettingManagementDbContext),
    typeof(ITextTemplateManagementDbContext),
    typeof(ILanguageManagementDbContext)
    )]
public class AdministrationServiceDbContext :
    AbpDbContext<AdministrationServiceDbContext>,
    IPermissionManagementDbContext,
    IFeatureManagementDbContext,
    ISettingManagementDbContext,
    ITextTemplateManagementDbContext,
    ILanguageManagementDbContext,
    IHasEventInbox,
    IHasEventOutbox
{
    public const string DbTablePrefix = "";
    public const string DbSchema = null;
    
    public const string DatabaseName = "Administration";
    
    public DbSet<IncomingEventRecord> IncomingEvents { get; set; }
    public DbSet<OutgoingEventRecord> OutgoingEvents { get; set; }
    
    /* These DbSet properties are coming from the Permission Management module */
    public DbSet<PermissionGroupDefinitionRecord> PermissionGroups { get; set; }
    public DbSet<PermissionDefinitionRecord> Permissions { get; set; }
    public DbSet<PermissionGrant> PermissionGrants { get; set; }

    /* These DbSet properties are coming from the Feature Management module */
    public DbSet<FeatureGroupDefinitionRecord> FeatureGroups { get; set; }
    public DbSet<FeatureDefinitionRecord> Features { get; set; }
    public DbSet<FeatureValue> FeatureValues { get; set; }
    
    /* These DbSet properties are coming from the Setting Management module */
    public DbSet<Setting> Settings { get; set; }
    public DbSet<SettingDefinitionRecord> SettingDefinitionRecords { get; set; }
    
    /* These DbSet properties are coming from the Language Management module */
    public DbSet<Language> Languages { get; set; }
    public DbSet<LanguageText> LanguageTexts { get; set; }
    public DbSet<LocalizationResourceRecord> LocalizationResources { get; set; }
    public DbSet<LocalizationTextRecord> LocalizationTexts { get; set; }

    /* These DbSet properties are coming from the Text Template Management module */
    public DbSet<TextTemplateContent> TextTemplateContents { get; set; }
    public DbSet<TextTemplateDefinitionRecord> TextTemplateDefinitionRecords { get; set; }
    public DbSet<TextTemplateDefinitionContentRecord> TextTemplateDefinitionContentRecords { get; set; }

    public AdministrationServiceDbContext(DbContextOptions<AdministrationServiceDbContext> options) 
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ConfigureEventInbox();
        builder.ConfigureEventOutbox();
        builder.ConfigurePermissionManagement();
        builder.ConfigureFeatureManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureLanguageManagement();
        builder.ConfigureTextTemplateManagement();
    }
}