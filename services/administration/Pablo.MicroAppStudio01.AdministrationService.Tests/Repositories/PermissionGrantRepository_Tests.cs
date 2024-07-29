using Shouldly;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.PermissionManagement;
using Xunit;
using Microsoft.EntityFrameworkCore;


namespace Pablo.MicroAppStudio01.AdministrationService.Tests.Repositories;

/* This is a demo test class to show how to test repositories.
 * You can delete this class freely.
 *
 * See https://docs.abp.io/en/abp/latest/Testing for more about automated tests.
 */

public class PermissionGrantRepository_Tests : AdministrationServiceIntegrationTestBase
{
    private readonly IPermissionGrantRepository _permissionGrantRepository;

    public PermissionGrantRepository_Tests()
    {
        _permissionGrantRepository = GetRequiredService<IPermissionGrantRepository>();
    }
    
    [Fact]
    public async Task Should_Get_Permissions_By_Role_Name()
    {
        var permissionGrants = await _permissionGrantRepository.GetListAsync(RolePermissionValueProvider.ProviderName, "admin");
        
        permissionGrants.ShouldNotBeNull();
        permissionGrants.Count.ShouldBeGreaterThanOrEqualTo(1);
    }
    
    
    [Fact]
    public async Task Should_Get_Permissions_By_Role_Name_With_Generic_Repository()
    {
        var genericPermissionGrantRepository = GetRequiredService<IRepository<PermissionGrant, Guid>>();
        
        /* We've used WithUnitOfWorkAsync because we are executing LINQ (Where(...).FirstOrDefaultAsync())
         * out of a repository class and that requires to have an active unit of work context.
         * See https://docs.abp.io/en/abp/latest/Testing#dealing-with-unit-of-work-in-integration-tests to learn why?
         */
        await WithUnitOfWorkAsync(async () =>
        {
            var queryable = await genericPermissionGrantRepository.GetQueryableAsync();
            var permissionGrant = await queryable.Where(x => x.ProviderKey == "admin").FirstOrDefaultAsync();

            permissionGrant.ShouldNotBeNull();
            permissionGrant.ProviderName.ShouldBe(RolePermissionValueProvider.ProviderName);
            permissionGrant.ProviderKey.ShouldBe("admin");
        });
    }
}