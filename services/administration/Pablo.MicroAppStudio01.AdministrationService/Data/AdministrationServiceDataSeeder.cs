using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.LanguageManagement.Data;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Pablo.MicroAppStudio01.AdministrationService.Data;

public class AdministrationServiceDataSeeder : ITransientDependency
{
    private readonly ILogger<AdministrationServiceDataSeeder> _logger;
    private readonly IPermissionDefinitionManager _permissionDefinitionManager;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly ICurrentTenant _currentTenant;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly LanguageManagementDataSeeder _languageManagementDataSeeder;

    public AdministrationServiceDataSeeder(
        ILogger<AdministrationServiceDataSeeder> logger,
        IPermissionDefinitionManager permissionDefinitionManager,
        IPermissionDataSeeder permissionDataSeeder,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager,
        LanguageManagementDataSeeder languageManagementDataSeeder)
    {
        _logger = logger;
        _permissionDefinitionManager = permissionDefinitionManager;
        _permissionDataSeeder = permissionDataSeeder;
        _currentTenant = currentTenant;
        _unitOfWorkManager = unitOfWorkManager;
        _languageManagementDataSeeder = languageManagementDataSeeder;
    }

    public async Task SeedAsync(Guid? tenantId = null)
    {
        using (_currentTenant.Change(tenantId))
        {
            using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
            {
                await SeedAdminPermissionsAsync(tenantId);
                await SeedLanguagesAsync(tenantId);
                await uow.CompleteAsync();
            }
        }
    }

    private async Task SeedLanguagesAsync(Guid? tenantId)
    {
        if (tenantId != null)
        {
            /* Language list is not multi-tenant */
            return;
        }

        await _languageManagementDataSeeder.SeedAsync();
    }

    private async Task SeedAdminPermissionsAsync(Guid? tenantId)
    {
        _logger.LogInformation($"Seeding admin permissions.");

        var multiTenancySide = tenantId == null
            ? MultiTenancySides.Host
            : MultiTenancySides.Tenant;

        var permissionNames = (await _permissionDefinitionManager.GetPermissionsAsync())
            .Where(p => p.MultiTenancySide.HasFlag(multiTenancySide))
            .Where(p => !p.Providers.Any() || p.Providers.Contains(RolePermissionValueProvider.ProviderName))
            .Select(p => p.Name)
            .ToArray();

        await _permissionDataSeeder.SeedAsync(
            RolePermissionValueProvider.ProviderName,
            "admin",
            permissionNames,
            tenantId
        );
    }
}