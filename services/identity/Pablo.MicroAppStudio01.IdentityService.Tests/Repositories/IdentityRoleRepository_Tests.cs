using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Xunit;
using Microsoft.EntityFrameworkCore;

namespace Pablo.MicroAppStudio01.IdentityService.Tests.Repositories;

/* This is a demo test class to show how to test repositories.
 * You can delete this class freely.
 *
 * See https://docs.abp.io/en/abp/latest/Testing for more about automated tests.
 */

public class IdentityRoleRepository_Tests : IdentityServiceIntegrationTestBase
{
    private readonly IIdentityRoleRepository _roleRepository;
    
    public IdentityRoleRepository_Tests()
    {
        _roleRepository = GetRequiredService<IIdentityRoleRepository>();
    }
    
    [Fact]
    public async Task Should_Get_Role_By_Name()
    {
        var role = await _roleRepository.FindByNormalizedNameAsync("ADMIN");
        
        role.ShouldNotBeNull();
        role.Name.ShouldBe("admin");
    }

    [Fact]
    public async Task Should_Get_Role_By_Name_With_Generic_Repository()
    {
        var genericRoleRepository = GetRequiredService<IRepository<IdentityRole, Guid>>();
        
        /* We've used WithUnitOfWorkAsync because we are executing LINQ (Where(...).FirstOrDefaultAsync())
         * out of a repository class and that requires to have an active unit of work context.
         * See https://docs.abp.io/en/abp/latest/Testing#dealing-with-unit-of-work-in-integration-tests to learn why?
         */
        await WithUnitOfWorkAsync(async () =>
        {
            var queryable = await genericRoleRepository.GetQueryableAsync();
            var role = await queryable.Where(x => x.Name == "admin").FirstOrDefaultAsync();

            role.ShouldNotBeNull();
            role.Name.ShouldBe("admin");
        });
    }
}