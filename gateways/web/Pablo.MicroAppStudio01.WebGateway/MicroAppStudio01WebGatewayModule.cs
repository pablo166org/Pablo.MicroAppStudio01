using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.AspNetCore.Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using Volo.Abp.Studio.Client.AspNetCore;
using Volo.Abp.Swashbuckle;
using Yarp.ReverseProxy.Configuration;
using Volo.Abp.AspNetCore.MultiTenancy;
using Volo.Abp.MultiTenancy;

namespace Pablo.MicroAppStudio01.WebGateway;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AbpAspNetCoreMultiTenancyModule),
    typeof(AbpStudioClientAspNetCoreModule)
)]
public class MicroAppStudio01WebGatewayModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        ConfigureCors(context, configuration);
        ConfigureMultiTenancy();
        ConfigureSwagger(context, configuration);
        ConfigureYarp(context, configuration);
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        var configuration = context.GetConfiguration();
        var proxyConfig = app.ApplicationServices.GetRequiredService<IProxyConfigProvider>().GetConfig();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseAbpStudioLink();
        app.UseCors();
        app.UseRouting();
        app.UseAuthorization();
        
        if (IsSwaggerEnabled(configuration))
        {
            app.UseSwagger();
            app.UseAbpSwaggerUI(options => { ConfigureSwaggerUI(proxyConfig, options, configuration); });
            app.UseRewriter(CreateSwaggerRewriteOptions());
        }

        app.UseAbpSerilogEnrichers();
        app.UseEndpoints(endpoints => endpoints.MapReverseProxy());
    }

    private void ConfigureMultiTenancy()
    {
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = true;
        });
    }

    private void ConfigureYarp(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"))
            .AddConfigFilter<WebGatewayConfigFilter>();
    }

    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        var corsOrigins = configuration["App:CorsOrigins"];
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                if (!corsOrigins.IsNullOrEmpty())
                {
                    builder
                        .WithOrigins(
                            corsOrigins!
                                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                                .Select(o => o.RemovePostFix("/"))
                                .ToArray()
                        )
                        .WithAbpExposedHeaders()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            });
        });
    }

    private static bool IsSwaggerEnabled(IConfiguration configuration)
    {
        return bool.Parse(configuration["Swagger:IsEnabled"] ?? "true");
    }

    private void ConfigureSwagger(ServiceConfigurationContext context, IConfiguration configuration)
    {
        if (IsSwaggerEnabled(configuration))
        {
            context.Services.AddAbpSwaggerGen();
        }
    }

    private static void ConfigureSwaggerUI(
        IProxyConfig proxyConfig,
        SwaggerUIOptions options,
        IConfiguration configuration)
    {
        foreach (var cluster in proxyConfig.Clusters)
        {
            options.SwaggerEndpoint($"/swagger-json/{cluster.ClusterId}/swagger/v1/swagger.json", $"{cluster.ClusterId} API");
        }

        options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
        options.OAuthScopes(
            "AdministrationService",
            "AuthServer",
            "SaasService",
            "AuditLoggingService",
            "ChatService",
            "FileManagementService",
            "IdentityService"
        );
    }
    
    private static RewriteOptions CreateSwaggerRewriteOptions()
    {
        var rewriteOptions = new RewriteOptions();
        rewriteOptions.AddRedirect("^(|\\|\\s+)$", "/swagger"); // Regex for "/" and "" (whitespace)
        return rewriteOptions;
    }
}