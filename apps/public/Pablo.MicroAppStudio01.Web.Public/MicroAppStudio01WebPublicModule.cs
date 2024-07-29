using System;
using System.IO;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Pablo.MicroAppStudio01.Web.Public.Localization;
using Pablo.MicroAppStudio01.Web.Public.Menus;
using StackExchange.Redis;
using Prometheus;
using Polly;
using Volo.Abp;
using Volo.Abp.Studio;
using Volo.Abp.Http.Client;
using Volo.Abp.Localization;
using Volo.Abp.AspNetCore.Authentication.OpenIdConnect;
using Volo.Abp.AspNetCore.Mvc.Client;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonX;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonX.Bundling;
using Volo.Abp.LeptonX.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Toolbars;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Http.Client.IdentityModel.Web;
using Volo.Abp.Http.Client.Web;
using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Account;
using Volo.Abp.Account.Pro.Public.Web.Shared;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Studio.Client.AspNetCore;

namespace Pablo.MicroAppStudio01.Web.Public;

[DependsOn(
    typeof(AbpAccountPublicHttpApiClientModule),
    typeof(AbpAccountPublicWebSharedModule),
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(AbpEventBusRabbitMqModule),
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(AbpDistributedLockingModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpStudioClientAspNetCoreModule),
    typeof(AbpAspNetCoreMvcUiLeptonXThemeModule),
    typeof(AbpAspNetCoreAuthenticationOpenIdConnectModule),
    typeof(AbpAspNetCoreMvcClientModule),
    typeof(AbpHttpClientWebModule),
    typeof(AbpHttpClientIdentityModelWebModule)
    )]
public class MicroAppStudio01WebPublicModule : AbpModule
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
        ConfigureMultiTenancy();
        ConfigureDistributedCache(configuration);
        ConfigureUrls(configuration);
        ConfigureAuthentication(context, configuration);
        ConfigureDataProtection(context, configuration, redis);
        ConfigureNavigation(configuration);
        ConfigureToolbar();
        ConfigureLeptonXTheme();
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
        app.UseMultiTenancy();
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
                typeof(MicroAppStudio01WebPublicResource),
                typeof(MicroAppStudio01WebPublicModule).Assembly
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
            options.FileSets.AddEmbedded<MicroAppStudio01WebPublicResource>(typeof(MicroAppStudio01WebPublicModule).Namespace);
        });
        
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<MicroAppStudio01WebPublicModule>(hostingEnvironment.ContentRootPath);
            });
        }

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<MicroAppStudio01WebPublicResource>("en")
                .AddVirtualJson("/Localization/MicroAppStudio01WebPublic");

            options.DefaultResourceType = typeof(MicroAppStudio01WebPublicResource);
        });
    }

    private void ConfigureBundling()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                LeptonXThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }
    
    private void ConfigureMultiTenancy()
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = true;
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

                options.Scope.Add("roles");
                options.Scope.Add("email");
                options.Scope.Add("phone");
                options.Scope.Add("AuthServer");
                options.Scope.Add("AdministrationService");
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
            options.MenuContributors.Add(new MicroAppStudio01PublicMenuContributor(configuration));
        });
    }
    
    private void ConfigureToolbar()
    {
        Configure<AbpToolbarOptions>(options =>
        {
            options.Contributors.Add(new MicroAppStudio01ToolbarContributor());
        });
    }
    
    private void ConfigureLeptonXTheme()
    {
        Configure<LeptonXThemeOptions>(options =>
        {
            options.DefaultStyle = LeptonXStyleNames.System;
        });
    }
}
