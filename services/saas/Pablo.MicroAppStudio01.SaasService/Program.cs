using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using Volo.Abp;
using Volo.Abp.Studio.Configuration;

namespace Pablo.MicroAppStudio01.SaasService;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            Log.Information($"Starting {GetCurrentAssemblyName()}");

            AbpStudioEnvironmentVariableLoader.Load();

            var builder = WebApplication.CreateBuilder(args);

            builder.Host
                .AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog((context, services, loggerConfiguration) =>
                {
                    var applicationName = services.GetRequiredService<IApplicationInfoAccessor>().ApplicationName;

                    loggerConfiguration
#if DEBUG
                        .MinimumLevel.Debug()
#else
                        .MinimumLevel.Information()
#endif
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("Application", applicationName)
                        .WriteTo.Async(c => c.Console())
                        .If(Convert.ToBoolean(context.Configuration["ElasticSearch:IsLoggingEnabled"]), c =>
                            c.WriteTo.Elasticsearch(
                                new ElasticsearchSinkOptions(new Uri(context.Configuration["ElasticSearch:Url"]!))
                                {
                                    AutoRegisterTemplate = true,
                                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6,
                                    IndexFormat = "MicroAppStudio01-log-{0:yyyy.MM}" 
                                })
                        )
#if DEBUG
                        .WriteTo.Async(c => c.File("Logs/logs.txt"))
#endif
                        .WriteTo.Async(c => c.AbpStudio(services));
                });

            await builder.AddApplicationAsync<MicroAppStudio01SaasServiceModule>();

            var app = builder.Build();

            await app.InitializeApplicationAsync();
            await app.RunAsync();

            Log.Information($"Stopped {GetCurrentAssemblyName()}");

            return 0;
        }
        catch (HostAbortedException)
        {
            /* Ignoring this exception because: https://github.com/dotnet/efcore/issues/29809#issuecomment-1345132260 */
            return 2;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{GetCurrentAssemblyName()} terminated unexpectedly!");
            Console.WriteLine(ex.ToString());
            Console.WriteLine(ex.StackTrace ?? "");

            Log.Fatal(ex, $"{GetCurrentAssemblyName()} terminated unexpectedly!");
            Log.Fatal(ex.Message);
            Log.Fatal(ex.StackTrace ?? "");
            return 1;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static string GetCurrentAssemblyName()
    {
        return typeof(Program).Assembly.GetName().Name!;
    }
}