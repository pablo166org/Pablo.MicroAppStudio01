using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Pablo.MicroAppStudio01.MicroserviceName.Navigation;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
{{~ if config.theme == "basic" ~}}
using Volo.Abp.AspNetCore.Components.Web.BasicTheme.Themes.Basic;
using Volo.Abp.AspNetCore.Components.WebAssembly.BasicTheme;
{{~ else if config.theme == "leptonx" ~}}
using Volo.Abp.AspNetCore.Components.Web.LeptonXTheme.Components;
using Volo.Abp.AspNetCore.Components.WebAssembly.LeptonXTheme;
using Volo.Abp.LeptonX.Shared;
{{~ end ~}}
using Volo.Abp.Autofac.WebAssembly;
using Volo.Abp.AutoMapper;
using Pablo.MicroAppStudio01.MicroserviceName.Localization;
using Volo.Abp.Localization;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;

namespace Pablo.MicroAppStudio01.MicroserviceName;

[DependsOn(
    {{~ if config.theme == "basic" ~}}
    typeof(AbpAspNetCoreComponentsWebAssemblyBasicThemeModule),
    {{~ else if config.theme == "leptonx" ~}}
    typeof(AbpAspNetCoreComponentsWebAssemblyLeptonXThemeModule),
    {{~ end ~}}    
    typeof(AbpAutofacWebAssemblyModule)
)]
public class MicroserviceNameModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var environment = context.Services.GetSingletonInstance<IWebAssemblyHostEnvironment>();
        var builder = context.Services.GetSingletonInstance<WebAssemblyHostBuilder>();

        ConfigureAuthentication(builder);
        ConfigureHttpClient(context, environment);
        ConfigureBlazorise(context);
        ConfigureRouter(context);
        ConfigureUI(builder);
        ConfigureNavigation(context);
        ConfigureAutoMapper(context);
        ConfigureLocalization();
        {{~ if config.theme == "leptonx" ~}}
        ConfigureLeptonXTheme();
        {{~ end ~}}
    }

    private void ConfigureLocalization()
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MicroserviceNameResource>(typeof(MicroserviceNameModule).Namespace);
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<MicroserviceNameResource>("en")
                .AddVirtualJson("/Localization/MicroserviceName");

            options.DefaultResourceType = typeof(MicroserviceNameResource);
        });
    }
    {{~ if config.theme == "leptonx" ~}}
    
    private void ConfigureLeptonXTheme()
    {
        Configure<LeptonXThemeOptions>(options =>
        {
            {{~ if config.theme_style == "system" ~}}
            options.DefaultStyle = LeptonXStyleNames.System;
            {{~ else if config.theme_style == "dim" ~}}
            options.DefaultStyle = LeptonXStyleNames.Dim;
            {{~ else if config.theme_style == "light" ~}}
            options.DefaultStyle = LeptonXStyleNames.Light;
            {{~ else if config.theme_style == "dark" ~}}
            options.DefaultStyle = LeptonXStyleNames.Dark;
            {{~ end ~}}
        });
    }
    {{~ end ~}}

    private void ConfigureRouter(ServiceConfigurationContext context)
    {
        Configure<AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(MicroserviceNameModule).Assembly;
        });
    }

    private void ConfigureNavigation(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new MicroserviceNameMenuContributor(context.Services.GetConfiguration()));
        });
    }

    private void ConfigureBlazorise(ServiceConfigurationContext context)
    {
        context.Services
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
    }

    private static void ConfigureAuthentication(WebAssemblyHostBuilder builder)
    {
        builder.Services.AddOidcAuthentication(options =>
        {
            builder.Configuration.Bind("AuthServer", options.ProviderOptions);
            
        });
    }

    private static void ConfigureUI(WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#ApplicationContainer");
    }

    private static void ConfigureHttpClient(ServiceConfigurationContext context, IWebAssemblyHostEnvironment environment)
    {
        context.Services.AddTransient(sp => new HttpClient
        {
            BaseAddress = new Uri(environment.BaseAddress)
        });
    }

    private void ConfigureAutoMapper(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<MicroserviceNameModule>();
        });
    }
}
