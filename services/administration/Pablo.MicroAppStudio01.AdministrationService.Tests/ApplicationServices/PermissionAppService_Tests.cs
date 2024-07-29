using Shouldly;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.PermissionManagement;
using Xunit;

namespace Pablo.MicroAppStudio01.AdministrationService.Tests.ApplicationServices;

/* This is a demo test class to show how to test application services.
 * You can delete this class freely.
 *
 * See https://docs.abp.io/en/abp/latest/Testing for more about automated tests.
 */

public class PermissionAppService_Tests : AdministrationServiceIntegrationTestBase
{
    private readonly IPermissionAppService _permissionAppService;

    public PermissionAppService_Tests()
    {
        _permissionAppService = GetRequiredService<IPermissionAppService>();
    }

    [Fact]
    public async Task Should_Get_Permissions()
    {
        var permissions= await _permissionAppService.GetAsync(RolePermissionValueProvider.ProviderName, "admin");
      
        permissions.ShouldNotBeNull();
        permissions.EntityDisplayName.ShouldBe("admin");
        permissions.Groups.Count.ShouldBeGreaterThanOrEqualTo(1);
        permissions.Groups.SelectMany(x => x.Permissions).Count().ShouldBeGreaterThanOrEqualTo(1);
    }
}