using System;
using System.Threading.Tasks;
using Localization.Resources.AbpUi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Pablo.MicroAppStudio01.MicroserviceName.Localization;
using Volo.Abp.UI.Navigation;

namespace Pablo.MicroAppStudio01.MicroserviceName.Navigation;

public class MicroserviceNameMenuContributor : IMenuContributor
{
    private readonly IConfiguration _configuration;

    public MicroserviceNameMenuContributor(IConfiguration configuration)
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
        var l = context.GetLocalizer<MicroserviceNameResource>();
    }

    private Task ConfigureUserMenuAsync(MenuConfigurationContext context)
    {
        return Task.CompletedTask;
    }
}
