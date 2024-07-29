using Shouldly;
using Volo.Abp.Identity;
using Xunit;

namespace Pablo.MicroAppStudio01.IdentityService.Tests.Entities;

/* This is a demo test class to show how to test entities.
 * You can delete this class freely.
 *
 * We didn't inherit from IdentityServiceTestBase because this is not an integration test,
 * this is a unit test. Unit tests run faster than integration tests.
 *
 * See https://docs.abp.io/en/abp/latest/Testing for more about automated tests.
 */

public class IdentityRole_Tests
{
    [Fact]
    public void Should_Change_Name()
    {
        // Arrange
        
        var role = new IdentityRole(Guid.NewGuid(), "test");
        role.Name.ShouldBe("test");
        
        // Act
        role.ChangeName("test2");
        
        // Assert
        role.Name.ShouldBe("test2");
        
        // Checking if the event is published
        var eventData = role
            .GetDistributedEvents()
            .Select(x => x.EventData)
            .OfType<IdentityRoleNameChangedEto>()
            .FirstOrDefault();
        
        eventData.ShouldNotBeNull();
        eventData.OldName.ShouldBe("test");
        eventData.Name.ShouldBe("test2");
    }
}