using Yarp.ReverseProxy.Configuration;

namespace Pablo.MicroAppStudio01.PublicGateway;

public class PublicGatewayConfigFilter : IProxyConfigFilter
{
    private readonly IConfiguration _configuration;

    public PublicGatewayConfigFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public ValueTask<ClusterConfig> ConfigureClusterAsync(ClusterConfig cluster, CancellationToken cancel)
    {
        return new ValueTask<ClusterConfig>(cluster);
    }

    public ValueTask<RouteConfig> ConfigureRouteAsync(RouteConfig route, ClusterConfig? cluster, CancellationToken cancel)
    {
        if (IsSwaggerRoute(route) && !IsSwaggerEnabled())
        {
            return new ValueTask<RouteConfig>(route with
            {
                /* Clearing transforms to disable swagger proxying to underlying service.
                 * Clients will get 404 since a non-transformed endpoint is not available in the target service.
                 * This is actually a workaround. Ideally, we should remove the route from the config,
                 * but YARP doesn't allow it.
                 */
                Transforms = new List<IReadOnlyDictionary<string, string>>()
            });
        }
        
        return new ValueTask<RouteConfig>(route);
    }

    private static bool IsSwaggerRoute(RouteConfig route)
    {
        return route.RouteId.EndsWith("Swagger");
    }

    private bool IsSwaggerEnabled()
    {
        return bool.Parse(_configuration["Swagger:IsEnabled"] ?? "true");
    }
}