using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Pablo.MicroAppStudio01.Blazor.Navigation;
using Volo.Abp.Account.Pro.Admin.Blazor.WebAssembly;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Components.Web.LeptonXTheme.Components;
using Volo.Abp.AspNetCore.Components.WebAssembly.LeptonXTheme;
using Volo.Abp.LeptonX.Shared;
using Volo.Abp.Autofac.WebAssembly;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity.Pro.Blazor.Server.WebAssembly;
using Pablo.MicroAppStudio01.Blazor.Localization;
using Volo.Abp.Localization;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.OpenIddict.Pro.Blazor.WebAssembly;
using Volo.Abp.LanguageManagement.Blazor.WebAssembly;
using Volo.Abp.AuditLogging.Blazor.WebAssembly;
using Pablo.MicroAppStudio01.AuditLoggingService;
using Volo.Abp.TextTemplateManagement.Blazor.WebAssembly;
using Volo.FileManagement.Blazor.WebAssembly;
using Pablo.MicroAppStudio01.FileManagementService;
using Volo.Chat.Blazor.WebAssembly;
using Pablo.MicroAppStudio01.ChatService;
using Volo.Saas.Host.Blazor.WebAssembly;
using Volo.Saas.Tenant.Blazor.WebAssembly;
using Pablo.MicroAppStudio01.SaasService;
using Pablo.MicroAppStudio01.IdentityService;
using Pablo.MicroAppStudio01.AdministrationService;

namespace Pablo.MicroAppStudio01.Blazor;

[DependsOn(
    typeof(AbpAutofacWebAssemblyModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyLeptonXThemeModule),
    typeof(AbpOpenIddictProBlazorWebAssemblyModule),
    typeof(AbpAuditLoggingBlazorWebAssemblyModule),
    typeof(MicroAppStudio01AuditLoggingServiceContractsModule),
    typeof(LanguageManagementBlazorWebAssemblyModule),
    typeof(TextTemplateManagementBlazorWebAssemblyModule),
    typeof(FileManagementBlazorWebAssemblyModule),
    typeof(MicroAppStudio01FileManagementServiceContractsModule),
    typeof(ChatBlazorWebAssemblyModule),
    typeof(MicroAppStudio01ChatServiceContractsModule),
    typeof(SaasHostBlazorWebAssemblyModule),
    typeof(SaasTenantBlazorWebAssemblyModule),
    typeof(MicroAppStudio01SaasServiceContractsModule),
    typeof(MicroAppStudio01IdentityServiceContractsModule),
    typeof(MicroAppStudio01AdministrationServiceContractsModule),
    typeof(AbpIdentityProBlazorWebAssemblyModule),
    typeof(AbpAccountAdminBlazorWebAssemblyModule)
)]
public class MicroAppStudio01BlazorModule : AbpModule
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
        ConfigureLeptonXTheme();
        ConfigureChat(builder);
    }

    private void ConfigureLocalization()
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MicroAppStudio01BlazorResource>(typeof(MicroAppStudio01BlazorModule).Namespace);
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<MicroAppStudio01BlazorResource>("en")
                .AddVirtualJson("/Localization/MicroAppStudio01Blazor");

            options.DefaultResourceType = typeof(MicroAppStudio01BlazorResource);
        });
    }
    
    private void ConfigureChat(WebAssemblyHostBuilder builder)
    {
        Configure<ChatBlazorWebAssemblyOptions>(options =>
        {
            options.SignalrUrl = builder.Configuration["RemoteServices:Chat:BaseUrl"];
        });
    }
    
    private void ConfigureLeptonXTheme()
    {
        Configure<LeptonXThemeOptions>(options =>
        {
            options.DefaultStyle = LeptonXStyleNames.System;
        });
    }

    private void ConfigureRouter(ServiceConfigurationContext context)
    {
        Configure<AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(MicroAppStudio01BlazorModule).Assembly;
        });
    }

    private void ConfigureNavigation(ServiceConfigurationContext context)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new MicroAppStudio01MenuContributor(context.Services.GetConfiguration()));
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
            options.UserOptions.NameClaim = "name";
            options.UserOptions.RoleClaim = "role";
            options.ProviderOptions.DefaultScopes.Add("roles");
            options.ProviderOptions.DefaultScopes.Add("email");
            options.ProviderOptions.DefaultScopes.Add("phone");
            options.ProviderOptions.DefaultScopes.Add("AuthServer");
            options.ProviderOptions.DefaultScopes.Add("IdentityService");
            options.ProviderOptions.DefaultScopes.Add("AdministrationService");
            options.ProviderOptions.DefaultScopes.Add("SaasService");
            options.ProviderOptions.DefaultScopes.Add("AuditLoggingService");
            options.ProviderOptions.DefaultScopes.Add("FileManagementService");
            options.ProviderOptions.DefaultScopes.Add("ChatService");
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
            options.AddMaps<MicroAppStudio01BlazorModule>();
        });
    }
}
