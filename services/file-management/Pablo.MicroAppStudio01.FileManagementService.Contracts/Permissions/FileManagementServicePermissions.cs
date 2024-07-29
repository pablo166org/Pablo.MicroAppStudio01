using Volo.Abp.Reflection;

namespace Pablo.MicroAppStudio01.FileManagementService.Permissions;

public class FileManagementServicePermissions
{
    public const string GroupName = "FileManagementService";

    public static string[] GetAll()
    {
        return ReflectionHelper.GetPublicConstantsRecursively(typeof(FileManagementServicePermissions));
    }
}