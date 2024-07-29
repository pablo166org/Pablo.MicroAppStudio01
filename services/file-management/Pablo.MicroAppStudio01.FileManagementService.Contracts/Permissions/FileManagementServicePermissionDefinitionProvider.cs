using Pablo.MicroAppStudio01.FileManagementService.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Pablo.MicroAppStudio01.FileManagementService.Permissions;

public class FileManagementServicePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        //var myGroup = context.AddGroup(FileManagementServicePermissions.GroupName);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FileManagementServiceResource>(name);
    }
}