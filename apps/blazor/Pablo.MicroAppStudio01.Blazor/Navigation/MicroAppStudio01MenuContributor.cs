using System;
using System.Threading.Tasks;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pablo.MicroAppStudio01.AdministrationService.Permissions;
using Pablo.MicroAppStudio01.Blazor.Localization;
using Volo.Abp.Account.AuthorityDelegation;
using Volo.Abp.Account.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Identity.Pro.Blazor.Navigation;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Users;
using Volo.Abp.OpenIddict.Pro.Blazor.Menus;
using Volo.Abp.AuditLogging.Blazor.Menus;
using Volo.Abp.LanguageManagement.Blazor.Menus;
using Volo.Abp.TextTemplateManagement.Blazor.Menus;
    
namespace Pablo.MicroAppStudio01.Blazor.Navigation;

public class MicroAppStudio01MenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public MicroAppStudio01MenuContributor(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
        else if (context.Menu.Name == StandardMenus.User)
        {
            await ConfigureUserMenuAsync(context);
        }
    }

    private static async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<MicroAppStudio01BlazorResource>();

        //Home
        context.Menu.AddItem(
            new ApplicationMenuItem( 
                MicroAppStudio01Menus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fa fa-home",
                order: 0
            )
        );

        //HostDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                MicroAppStudio01Menus.HostDashboard,
                l["Menu:Dashboard"],
                "/HostDashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(AdministrationServicePermissions.Dashboard.Host)
        );

        //TenantDashboard
        context.Menu.AddItem(
            new ApplicationMenuItem(
                MicroAppStudio01Menus.TenantDashboard,
                l["Menu:Dashboard"],
                "/Dashboard",
                icon: "fa fa-chart-line",
                order: 2
            ).RequirePermissions(AdministrationServicePermissions.Dashboard.Tenant)
        );

        //Administration
        var administration = context.Menu.GetAdministration();
        administration.Order = 4;        
        
        //Administration->Identity
        administration.SetSubItemOrder(IdentityProMenus.GroupName, 1);
        
        //Administration->OpenIddict
        administration.SetSubItemOrder(OpenIddictProMenus.GroupName, 2);
        
        //Administration->Language Management
        administration.SetSubItemOrder(LanguageManagementMenus.GroupName, 3);

        //Administration->Text Template Management
        administration.SetSubItemOrder(TextTemplateManagementMenus.GroupName, 4);

        //Administration->Audit Logs
        administration.SetSubItemOrder(AbpAuditLoggingMenus.GroupName, 5);

        //Administration->Settings
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 6);
    }

    private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();
        var options = context.ServiceProvider.GetRequiredService<IOptions<AbpAccountAuthorityDelegationOptions>>();

        var uiResource = context.GetLocalizer<AbpUiResource>();
        var accountResource = context.GetLocalizer<AccountResource>();

        var authServerUrl = _configuration["AuthServer:Authority"] ?? "~";
        var returnUrl = _configuration["App:SelfUrl"] ?? "";
        
        if (currentUser.FindImpersonatorUserId() == null && options.Value.EnableDelegatedImpersonation)
        {
            context.Menu.AddItem(new ApplicationMenuItem("Account.AuthorityDelegation", accountResource["AuthorityDelegation"], url: "javascript:void(0)", icon: "fa fa-users"));
        }
        
        context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountResource["MyAccount"], $"{authServerUrl.EnsureEndsWith('/')}Account/Manage?returnUrl={returnUrl}", icon: "fa fa-cog", order: 1000, null, "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.SecurityLogs", accountResource["MySecurityLogs"], $"{authServerUrl.EnsureEndsWith('/')}Account/SecurityLogs?returnUrl={returnUrl}", icon: "fa fa-user-shield", target: "_blank").RequireAuthenticated());

        return Task.CompletedTask;
    }
}
