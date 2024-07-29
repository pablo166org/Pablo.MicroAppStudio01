# Pablo.MicroAppStudio01.MicroserviceName

## Integrating to other services

### Replacing placeholders

#### Configuring AppSettings

Open `appsettings.json` file and you will see that:

`Administration` & `AbpBlobStoring` connection strings are not placed. You can copy them directly from the other services.

`StringEncryption>DefaultPassPhrase` field is not placed. You can copy the actual value directly from the other services.

`App>CorsOrigins` & `AuthServer>Authority` & `Authority>MetaAddress`  fields are not placed with related ports. You can copy the actual value directly from the other services.

### Configuring authority

#### Adding new service to OpenIddictDataSeeder

You need to introduce the new service to OpenIddict. To do that, open `OpenIddictDataSeeder` in `Identity` service, and add necessary information like other services.

```csharp
    private async Task CreateApiScopesAsync()
    {
        await CreateScopesAsync("AuthServer");
        await CreateScopesAsync("IdentityService");
        await CreateScopesAsync("AdministrationService");
        await CreateScopesAsync("MicroserviceName"); // new service
    }
    
    private async Task CreateSwaggerClientsAsync()
    {
        await CreateSwaggerClientAsync("SwaggerTestUI", new[]
        {
            "AuthServer",
            "IdentityService",
            "AdministrationService",
            "MicroserviceName" // new service
        });
    }
```
```csharp
    private async Task CreateSwaggerClientAsync(string clientId, string[] scopes)
    {
        ...
        ...
        ...
        var administrationServiceRootUrl = _configuration["OpenIddict:Resources:AdministrationService:RootUrl"]!.TrimEnd('/');
        var microserviceNameServiceRootUrl = _configuration["OpenIddict:Resources:MicroserviceName:RootUrl"]!.TrimEnd('/'); // new service

        await CreateOrUpdateApplicationAsync(
            name: clientId,
            type:  OpenIddictConstants.ClientTypes.Public,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: "Swagger Test Client",
            secret: null,
            grantTypes: new List<string>
            {
                OpenIddictConstants.GrantTypes.AuthorizationCode,
            },
            scopes: commonScopes.Union(scopes).ToList(),
            redirectUris: new List<string> {
                $"{webGatewaySwaggerRootUrl}/swagger/oauth2-redirect.html",
                $"{authServerRootUrl}/swagger/oauth2-redirect.html",
                $"{identityServiceRootUrl}/swagger/oauth2-redirect.html",
                $"{administrationServiceRootUrl}/swagger/oauth2-redirect.html",
                $"{microserviceNameServiceRootUrl}/swagger/oauth2-redirect.html", // new service
            }
        );
    }
```
```csharp
        var webClientRootUrl = _configuration["OpenIddict:Applications:Web:RootUrl"]!.EnsureEndsWith('/');
        await CreateOrUpdateApplicationAsync(
            name: "Web",
            type: OpenIddictConstants.ClientTypes.Confidential,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: "Web Client",
            secret: "1q2w3e*",
            grantTypes: new List<string> //Hybrid flow
            {
                OpenIddictConstants.GrantTypes.AuthorizationCode,
                OpenIddictConstants.GrantTypes.Implicit
            },
            scopes: commonScopes.Union(new[] {"AuthServer", "IdentityService", "AdministrationService", "MicroserviceName"}).ToList(), // added new service to the scopes
            redirectUris: new List<string> { $"{webClientRootUrl}signin-oidc" },
            postLogoutRedirectUris: new List<string>() { $"{webClientRootUrl}signout-callback-oidc" }
```

And in the `appsettings.json` file of the `Identity` service, add the new service to OpenIddict resources:
```json
  "OpenIddict": {
    "Applications": {
      ...
    },
    "Resources": {
      ...
      "AdministrationService": {
        "RootUrl": "http://localhost:****"
      },
      "MicroserviceName": {
        "RootUrl": "http://localhost:{{ config.launch_ports["service"] }}"
      }
    }
  }
```

#### Adding CORS to AuthServer

You need to add the port of the new service to AuthServer CorsOrigins and RedirectAllowedUrls:

```json
  "App": {
    "SelfUrl": "http://localhost:***",
    "CorsOrigins": "...... http://localhost:{{ config.launch_ports["service"] }}",
    "EnablePII": false,
    "RedirectAllowedUrls": "...... http://localhost:{{ config.launch_ports["service"] }}"
  },
```

### Configuring gateway

To redirect the requests, the new service should be introduced to the related gateway. For example, the following changes are needed for `webgateway`:

- Open `appsettings.json` file of webgateway service, Add the new service to clusters and routes:

```json
   "ReverseProxy": {
    "Routes": {
      ...
      "MicroserviceName": {
        "ClusterId": "MicroserviceName",
        "Match": {
          "Path": "/api/microservicename/{**catch-all}"
        }
      },
      "MicroserviceNameSwagger": {
        "ClusterId": "MicroserviceName",
        "Match": {
          "Path": "/swagger-json/MicroserviceName/swagger/v1/swagger.json"
        },
        "Transforms": [
          { "PathRemovePrefix": "/swagger-json/MicroserviceName" }
        ]
      }
    },
    "Clusters": {
      ...
      "MicroserviceName": {
        "Destinations": {
          "MicroserviceName": {
            "Address": "http://localhost:{{ config.launch_ports["service"] }}/"
          }
        }
      }
    }
```

- Open `MicroAppStudio01WebGatewayModule` file and add a new OAuthScope for the new service:

```csharp
        options.OAuthScopes(
            "AdministrationService",
            "AuthServer",
            "IdentityService",
            "MicroserviceName" // new service
        );
```

You also need to add scope for the new service on the UI services.

### Configuring UI service

#### MVC

You  need to add scope for the web project:

````json
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
                ...
                options.Scope.Add("AuthServer");
                options.Scope.Add("IdentityService");
                options.Scope.Add("AdministrationService");
                options.Scope.Add("MicroserviceName"); // new servie
            });
````

## Add new service to Run Profiles

To run the new service using ABP Studio, You need to add it to a Run Profile. Go to Solution Runner section on ABP Studio, there you can add the new service as a C# Application.

## Configuration for Docker

If you want to monitor the new microservice with Prometheus when you debug the solution, you should add the new microservice to the `prometheus.yml` file in the `etc/docker/prometheus` folder. You can copy the configurations from the existing microservices and modify them according to the new microservice. Below is an example of the `prometheus.yml` file for the `microservicename` microservice.

```
  ...
    static_configs:
    - targets: ['host.docker.internal:***']
  - job_name: 'microservicename'
    scheme: http
    metrics_path: 'metrics'
    static_configs:
    - targets: ['host.docker.internal:{{ config.launch_ports["service"] }}']
  - job_name: 'web'
    scheme: http
    metrics_path: 'metrics'
  ...
```

## Configuration for Kubernetes

A Helm chart is needed for the new service. After you create it, you can open Solution Tunnel section on ABP Studio and refresh the main chart to see it on the UI. 