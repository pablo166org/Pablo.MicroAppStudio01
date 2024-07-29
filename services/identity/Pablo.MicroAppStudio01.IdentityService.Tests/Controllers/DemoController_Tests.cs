using Shouldly;
using Xunit;

namespace Pablo.MicroAppStudio01.IdentityService.Tests.Controllers;

/* This is a demo test class to show how to test HTTP API controllers.
 * You can delete this class freely.
 *
 * See https://docs.abp.io/en/abp/latest/Testing for more about automated tests.
 */

public class DemoController_Tests : IdentityServiceIntegrationTestBase
{
    [Fact]
    public async Task HelloWorld()
    {
        var response = await GetResponseAsStringAsync("/api/identity/demo/hello");
        response.ShouldBe("Hello World!");
    }
}