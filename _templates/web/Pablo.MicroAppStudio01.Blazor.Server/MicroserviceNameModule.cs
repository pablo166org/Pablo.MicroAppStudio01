using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Pablo.MicroAppStudio01.MicroserviceName.Localization;
using Pablo.MicroAppStudio01.MicroserviceName.Navigation;
using Polly;
using Prometheus;
using StackExchange.Redis;
using Volo.Abp;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Components.Web.Theming.Routing;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
{{~ if config.theme == "basic" ~}}
using Volo.Abp.AspNetCore.Components.Server.BasicTheme;
using Volo.Abp.AspNetCore.Components.Server.BasicTheme.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Components.Web.BasicTheme.Themes.Basic;
{{~ else if config.theme == "leptonx" ~}}
using Volo.Abp.AspNetCore.Components.Server.LeptonXTheme;
using Volo.Abp.AspNetCore.Components.Server.LeptonXTheme.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonX;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonX.Bundling;
using Volo.Abp.LeptonX.Shared;
{{~ end ~}}
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Http.Client.Web;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.UI.Navigation;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.Localization;
using Volo.Abp.VirtualFileSystem;

namespace Pablo.MicroAppStudio01.MicroserviceName;

[DependsOn(
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpEventBusRabbitMqModule),
    typeof(AbpAspNetCoreMvcClientModule),
    typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule),
    typeof(AbpHttpClientWebModule),
    typeof(AbpHttpClientIdentityModelWebModule),
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule),
    {{~ if config.theme == "basic" ~}}
    typeof(AbpAspNetCoreComponentsServerBasicThemeModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    {{~ else if config.theme == "leptonx" ~}}
    typeof(AbpAspNetCoreComponentsServerLeptonXThemeModule),
    typeof(AbpAspNetCoreMvcUiLeptonXThemeModule),
    {{~ end ~}}
    typeof(AbpStudioClientAspNetCoreModule)
)]
public class MicroserviceNameModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigureDataAnnotations(context);
        PreConfigureHttpClient();
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        var redis = CreateRedisConnection(configuration);
       
        ConfigurePII(configuration);
        ConfigureLocalization(hostingEnvironment);
        ConfigureBundling();
        ConfigureDistributedCache(configuration);
        ConfigureUrls(configuration);
        ConfigureAuthentication(context, configuration);
        ConfigureDataProtection(context, configuration, redis);
        ConfigureNavigation(configuration);
        ConfigureToolbar();
        {{~ if config.theme == "leptonx" ~}}
        ConfigureLeptonXTheme();
        {{~ end ~}}
        ConfigureBlazorise(context);
        ConfigureRouter(context); 
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (!env.IsDevelopment())
        {
            app.Use((ctx, next) =>
            {
                /* This application should act like it is always called as HTTPS.
                 * Because it will work in a HTTPS url in production,
                 * but the HTTPS is stripped out in Ingress controller.
                 */
                ctx.Request.Scheme = "https";
                return next();
            });
        }

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAbpRequestLocalization();

        if (!env.IsDevelopment())
        {
            app.UseErrorPage();
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });
        
        if (env.IsDevelopment())
        {
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None,
                Secure = CookieSecurePolicy.Always
            });
        }
        
        app.UseCorrelationId();
        app.UseAbpSecurityHeaders();
        app.UseStaticFiles();
        app.UseAbpStudioLink();
        app.UseRouting();
        app.UseHttpMetrics();
        app.UseAuthentication();
        {{~ if config.multi_tenancy ~}}
        app.UseMultiTenancy();
        {{~ end ~}}
        app.UseAbpSerilogEnrichers();
        app.UseAuthorization();
        app.UseConfiguredEndpoints(endpoints =>
        {
            endpoints.MapMetrics();
        });
    }
    
    private static void PreConfigureDataAnnotations(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(MicroserviceNameResource),
                typeof(MicroserviceNameModule).Assembly
            );
        });
    }
    
    private void PreConfigureHttpClient()
    {
        PreConfigure<AbpHttpClientBuilderOptions>(options =>
        {
            options.ProxyClientBuildActions.Add((remoteServiceName, clientBuilder) =>
            {
                clientBuilder.AddTransientHttpErrorPolicy(policyBuilder =>
                    policyBuilder.WaitAndRetryAsync(
                        4,
                        i => TimeSpan.FromSeconds(Math.Pow(2, i))
                    )
                );
            });
        });
    }
    
    private ConnectionMultiplexer CreateRedisConnection(IConfiguration configuration)
    {
        return ConnectionMultiplexer.Connect(configuration["Redis:Configuration"]);
    }
    
    private void ConfigurePII(IConfiguration configuration)
    {
        if (Convert.ToBoolean(configuration["App:EnablePII"]))
        {
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;
        }
    }
    
    private void ConfigureLocalization(IWebHostEnvironment hostingEnvironment)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<MicroserviceNameResource>(typeof(MicroserviceNameModule).Namespace);
        });
        
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<MicroserviceNameModule>(hostingEnvironment.ContentRootPath);
            });
        }

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<MicroserviceNameResource>("en")
                .AddVirtualJson("/Localization/MicroserviceName");

            options.DefaultResourceType = typeof(MicroserviceNameResource);
        });
    }
    
    private void ConfigureBundling()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            // MVC UI
            options.StyleBundles.Configure(
                {{~ if config.theme == "basic" ~}}
                BasicThemeBundles.Styles.Global,
                {{~ else if config.theme == "leptonx" ~}}
                LeptonXThemeBundles.Styles.Global,
                {{~ end ~}}
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );

            // Blazor UI
            options.StyleBundles.Configure(
                {{~ if config.theme == "basic" ~}}
                BlazorBasicThemeBundles.Styles.Global,
                {{~ else if config.theme == "leptonx" ~}}
                BlazorLeptonXThemeBundles.Styles.Global,
                {{~ end ~}}
                bundle =>
                {
                    bundle.AddFiles("/blazor-global-styles.css");
                }
            );
        });
    }
    
    private void ConfigureDistributedCache(IConfiguration configuration)
    {
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = configuration["AbpDistributedCache:KeyPrefix"]!;
        });
    }
    
    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        }); 
    }
    
    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies", options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
            })
            .AddAbpOpenIdConnect("oidc", options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                
                options.ClientId = configuration["AuthServer:ClientId"];
                options.ClientSecret = configuration["AuthServer:ClientSecret"];

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                
                options.TokenValidationParameters.ValidIssuers = new[]
                {
                    configuration["AuthServer:Authority"].EnsureEndsWith('/'),
                    configuration["AuthServer:MetaAddress"].EnsureEndsWith('/')
                }; 

            });
        
        if (Convert.ToBoolean(configuration["AuthServer:IsOnK8s"]))
        {
            context.Services.Configure<OpenIdConnectOptions>("oidc", options =>
            {
                options.MetadataAddress = configuration["AuthServer:MetaAddress"].EnsureEndsWith('/') + ".well-known/openid-configuration";

                var previousOnRedirectToIdentityProvider = options.Events.OnRedirectToIdentityProvider;
                options.Events.OnRedirectToIdentityProvider = async ctx =>
                {
                    // Intercept the redirection so the browser navigates to the right URL in your host
                    ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"].EnsureEndsWith('/') + "connect/authorize";
                    if (previousOnRedirectToIdentityProvider != null)
                    {
                        await previousOnRedirectToIdentityProvider(ctx);
                    }
                };
                var previousOnRedirectToIdentityProviderForSignOut = options.Events.OnRedirectToIdentityProviderForSignOut;
                options.Events.OnRedirectToIdentityProviderForSignOut = async ctx =>
                {
                    // Intercept the redirection for signout so the browser navigates to the right URL in your host
                    ctx.ProtocolMessage.IssuerAddress = configuration["AuthServer:Authority"].EnsureEndsWith('/') + "connect/logout";
                    if (previousOnRedirectToIdentityProviderForSignOut != null)
                    {
                        await previousOnRedirectToIdentityProviderForSignOut(ctx);
                    }
                };
            });
        }
    }
    
    private static void ConfigureDataProtection(ServiceConfigurationContext context, IConfiguration configuration, ConnectionMultiplexer redis)
    {
        context.Services
            .AddDataProtection()
            .SetApplicationName(configuration["DataProtection:ApplicationName"]!)
            .PersistKeysToStackExchangeRedis(redis, configuration["DataProtection:Keys"]);
    }
    
    private void ConfigureNavigation(IConfiguration configuration)
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new MicroserviceNameMenuContributor(configuration));
        });
    }
    
    private void ConfigureToolbar()
    {
        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new MicroserviceNameToolbarContributor());
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
    
    private void ConfigureBlazorise(ServiceConfigurationContext context)
    {
        context.Services
            .AddBootstrap5Providers()
            .AddFontAwesomeIcons();
    }
    
    private void ConfigureRouter(ServiceConfigurationContext context)
    {
        Configure<AbpRouterOptions>(options =>
        {
            options.AppAssembly = typeof(MicroserviceNameModule).Assembly;
        });
    }
}
