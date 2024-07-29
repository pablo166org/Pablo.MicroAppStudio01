using Pablo.MicroAppStudio01.AdministrationService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Pablo.MicroAppStudio01.AdministrationService.Permissions;

public class AdministrationServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var administrationGroup = context.AddGroup(AdministrationServicePermissions.GroupName, L("AdministrationService"));

        administrationGroup.AddPermission(AdministrationServicePermissions.Dashboard.Host, L("Permission:Dashboard"), MultiTenancySides.Host);
        administrationGroup.AddPermission(AdministrationServicePermissions.Dashboard.Tenant, L("Permission:Dashboard"), MultiTenancySides.Tenant);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AdministrationServiceResource>(name);
    }
}