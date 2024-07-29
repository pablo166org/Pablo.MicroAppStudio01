using Shouldly;
using Volo.Abp.PermissionManagement;
using Xunit;

namespace Pablo.MicroAppStudio01.AdministrationService.Tests.Entities;

/* This is a demo test class to show how to test entities.
 * You can delete this class freely.
 *
 * We didn't inherit from AdministrationServiceIntegrationTestBase because this is not an integration test,
 * this is a unit test. Unit tests run faster than integration tests.
 *
 * See https://docs.abp.io/en/abp/latest/Testing for more about automated tests.
 */

public class PermissionDefinitionRecord_Tests
{
    [Fact]
    public void Should_Change_Name()
    {
        // Arrange
        var permission = new PermissionDefinitionRecord(
            Guid.NewGuid(),
            "test",
            "test",
            null,
            "test"
        );
        permission.Name.ShouldBe("test");
        
        // Act
        permission.Patch(new PermissionDefinitionRecord(
            Guid.NewGuid(),
            "test",
            "test2",
            null,
            "test"));
        
        // Assert
        permission.Name.ShouldBe("test2");
    } 
}