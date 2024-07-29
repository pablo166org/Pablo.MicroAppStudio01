using System;
using System.Threading.Tasks;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.Configuration;
using Pablo.MicroAppStudio01.Web.Public.Localization;
using Volo.Abp.Account.Localization;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;

namespace Pablo.MicroAppStudio01.Web.Public.Menus;

public class MicroAppStudio01PublicMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public MicroAppStudio01PublicMenuContributor(IConfiguration configuration)
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

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<MicroAppStudio01WebPublicResource>();

        // Home
        context.Menu.AddItem(
            new ApplicationMenuItem(
                MicroAppStudio01PublicMenus.HomePage,
                l["Menu:HomePage"],
                "~/",
                icon: "fa fa-home",
                order: 1
            )
        );

        // ArticleSample
        context.Menu.AddItem(
            new ApplicationMenuItem(
                MicroAppStudio01PublicMenus.ArticleSample,
                l["Menu:ArticleSample"],
                "~/article-sample",
                icon: "fa fa-file-signature",
                order: 2
                )
        );

        // Contact Us
        context.Menu.AddItem(
            new ApplicationMenuItem(
                MicroAppStudio01PublicMenus.ContactUs,
                l["Menu:ContactUs"],
                "~/contact-us",
                icon: "fa fa-phone",
                order: 3
                )
        );

        return Task.CompletedTask;
    }

    private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        var uiResource = context.GetLocalizer<AbpUiResource>();
        var accountResource = context.GetLocalizer<AccountResource>();
        
        var authServerUrl = _configuration["AuthServer:Authority"] ?? "~";
        var returnUrl = _configuration["App:SelfUrl"] ?? "";
       
        context.Menu.AddItem(new ApplicationMenuItem("Account.Manage", accountResource["MyAccount"], $"{authServerUrl.EnsureEndsWith('/')}Account/Manage?returnUrl={returnUrl}", icon: "fa fa-cog", order: 1000, null, "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.SecurityLogs", accountResource["MySecurityLogs"], $"{authServerUrl.EnsureEndsWith('/')}Account/SecurityLogs?returnUrl={returnUrl}", icon: "fa fa-user-shield", target: "_blank").RequireAuthenticated());
        context.Menu.AddItem(new ApplicationMenuItem("Account.Logout", uiResource["Logout"], url: "~/Account/Logout", icon: "fa fa-power-off", order: int.MaxValue - 1000).RequireAuthenticated());

        return Task.CompletedTask;
    }
}
