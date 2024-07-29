{{~
    if config.database_provider != "mongodb"
    helpers.delete_this_file
    else
    helpers.rename_this_file("MicroserviceNameDbContext.cs")
    end
~}}
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MongoDB;
using Volo.Abp.MongoDB.DistributedEvents;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.MongoDB;
using Volo.Abp.LanguageManagement;
using Volo.Abp.LanguageManagement.MongoDB;
using Volo.Abp.LanguageManagement.External;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.MongoDB;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.MongoDB;

namespace Pablo.MicroAppStudio01.MicroserviceName.Data;

[ConnectionStringName(DatabaseName)]
[ReplaceDbContext(
    typeof(IPermissionManagementMongoDbContext),
    typeof(IFeatureManagementMongoDbContext),
    typeof(ISettingManagementMongoDbContext),
    typeof(ILanguageManagementMongoDbContext)
)]
public class MicroserviceNameDbContext :
    AbpMongoDbContext,
    IPermissionManagementMongoDbContext,
    IFeatureManagementMongoDbContext,
    ISettingManagementMongoDbContext,
    ILanguageManagementMongoDbContext,
    IHasEventInbox,
    IHasEventOutbox
{
    public const string DatabaseName = "MicroserviceName";
    
    public IMongoCollection<IncomingEventRecord> IncomingEvents => Collection<IncomingEventRecord>();
    public IMongoCollection<OutgoingEventRecord> OutgoingEvents => Collection<OutgoingEventRecord>();
    
    /* These Collection properties are coming from the Permission Management module */
    public IMongoCollection<PermissionGroupDefinitionRecord> PermissionGroups => Collection<PermissionGroupDefinitionRecord>();
    public IMongoCollection<PermissionDefinitionRecord> Permissions => Collection<PermissionDefinitionRecord>();
    public IMongoCollection<PermissionGrant> PermissionGrants => Collection<PermissionGrant>();

    /* These Collection properties are coming from the Feature Management module */
    public IMongoCollection<FeatureGroupDefinitionRecord> FeatureGroups => Collection<FeatureGroupDefinitionRecord>();
    public IMongoCollection<FeatureDefinitionRecord> Features => Collection<FeatureDefinitionRecord>();
    public IMongoCollection<FeatureValue> FeatureValues => Collection<FeatureValue>();
    
    /* These Collection properties are coming from the Setting Management module */
    public IMongoCollection<Setting> Settings => Collection<Setting>();
    public IMongoCollection<SettingDefinitionRecord> SettingDefinitionRecords => Collection<SettingDefinitionRecord>();
    
    /* These Collection properties are coming from the Language Management module */
    public IMongoCollection<Language> Languages => Collection<Language>();
    public IMongoCollection<LanguageText> LanguageTexts => Collection<LanguageText>();
    public IMongoCollection<LocalizationResourceRecord> LocalizationResources => Collection<LocalizationResourceRecord>();
    public IMongoCollection<LocalizationTextRecord> LocalizationTexts => Collection<LocalizationTextRecord>();

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);
        
        modelBuilder.ConfigureEventInbox();
        modelBuilder.ConfigureEventOutbox();
        modelBuilder.ConfigurePermissionManagement();
        modelBuilder.ConfigureFeatureManagement();
        modelBuilder.ConfigureSettingManagement();
        modelBuilder.ConfigureLanguageManagement();
    }
}