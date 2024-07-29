using Shouldly;
using Volo.Abp.Identity;
using Xunit;

namespace Pablo.MicroAppStudio01.IdentityService.Tests.ApplicationServices;

/* This is a demo test class to show how to test application services.
 * You can delete this class freely.
 *
 * See https://docs.abp.io/en/abp/latest/Testing for more about automated tests.
 */

public class IdentityRoleAppService_Tests : IdentityServiceIntegrationTestBase
{
    private readonly IIdentityRoleAppService _roleAppService;

    public IdentityRoleAppService_Tests()
    {
        _roleAppService = GetRequiredService<IIdentityRoleAppService>();
    }

    [Fact]
    public async Task Should_Get_Roles()
    {
        var roles = await _roleAppService.GetListAsync(new GetIdentityRoleListInput());
        roles.TotalCount.ShouldBeGreaterThan(0);
        roles.Items.Count.ShouldBeGreaterThan(0);
        roles.Items.ShouldContain(x => x.Name == "admin");
    }
}