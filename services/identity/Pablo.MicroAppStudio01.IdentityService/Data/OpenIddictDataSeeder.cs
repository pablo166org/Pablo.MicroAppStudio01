using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using Volo.Abp;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

namespace Pablo.MicroAppStudio01.IdentityService.Data;

public class OpenIddictDataSeeder : ITransientDependency
{
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant _currentTenant;
    private readonly IAbpApplicationManager _applicationManager;
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly IOpenIddictApplicationRepository _applicationRepository;
    private readonly IOpenIddictScopeRepository _scopeRepository;
    private readonly OpenIddictCoreOptions _openIddictCoreOptions;

    public OpenIddictDataSeeder(
        IConfiguration configuration,
        ICurrentTenant currentTenant,
        IAbpApplicationManager applicationManager,
        IOpenIddictScopeManager scopeManager,
        IPermissionDataSeeder permissionDataSeeder,
        IOpenIddictApplicationRepository applicationRepository,
        IOpenIddictScopeRepository scopeRepository,
        IOptions<OpenIddictCoreOptions> openIddictCoreOptions)
    {
        _configuration = configuration;
        _currentTenant = currentTenant;
        _applicationManager = applicationManager;
        _scopeManager = scopeManager;
        _permissionDataSeeder = permissionDataSeeder;
        _applicationRepository = applicationRepository;
        _scopeRepository = scopeRepository;
        _openIddictCoreOptions = openIddictCoreOptions.Value;
    }

    [UnitOfWork(isTransactional: true)]
    public virtual async Task SeedAsync()
    {
        using (_currentTenant.Change(null))
        {
            var oldCacheValue = _openIddictCoreOptions.DisableEntityCaching;
            _openIddictCoreOptions.DisableEntityCaching = true;
            try
            {
                await CreateApiScopesAsync();
                await CreateClientsAsync();
                await CreateSwaggerClientsAsync();
            }
            finally
            {
                _openIddictCoreOptions.DisableEntityCaching = oldCacheValue;
            }
        }
    }

    private async Task CreateApiScopesAsync()
    {
        await CreateScopesAsync("AuthServer");
        await CreateScopesAsync("IdentityService");
        await CreateScopesAsync("AdministrationService");
        await CreateScopesAsync("SaasService");
        await CreateScopesAsync("AuditLoggingService");
        await CreateScopesAsync("FileManagementService");
        await CreateScopesAsync("ChatService");
    }

    private async Task CreateSwaggerClientsAsync()
    {
        await CreateSwaggerClientAsync("SwaggerTestUI", new[]
        {
            "AuthServer",
            "IdentityService",
            "SaasService",
            "AuditLoggingService",
            "FileManagementService",
            "ChatService",
            "AdministrationService"
        });
    }

    private async Task CreateSwaggerClientAsync(string clientId, string[] scopes)
    {
        var commonScopes = new List<string>
        {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        };

        // Swagger Client
        //TODO: Ensure that all root urls are configured in the appsettings.json file properly
        var webGatewaySwaggerRootUrl = _configuration["OpenIddict:Applications:WebGateway:RootUrl"]!.TrimEnd('/'); 
        var publicGatewaySwaggerRootUrl = _configuration["OpenIddict:Applications:PublicGateway:RootUrl"]!.TrimEnd('/');
        var authServerRootUrl = _configuration["OpenIddict:Resources:AuthServer:RootUrl"]!.TrimEnd('/');
        var identityServiceRootUrl = _configuration["OpenIddict:Resources:IdentityService:RootUrl"]!.TrimEnd('/');
        var administrationServiceRootUrl = _configuration["OpenIddict:Resources:AdministrationService:RootUrl"]!.TrimEnd('/');
        var saasServiceServiceRootUrl = _configuration["OpenIddict:Resources:SaasService:RootUrl"]!.TrimEnd('/');
        var auditLoggingServiceServiceRootUrl = _configuration["OpenIddict:Resources:AuditLoggingService:RootUrl"]!.TrimEnd('/'); 
        var fileManagementServiceServiceRootUrl = _configuration["OpenIddict:Resources:FileManagementService:RootUrl"]!.TrimEnd('/'); 
        var chatServiceServiceRootUrl = _configuration["OpenIddict:Resources:ChatService:RootUrl"]!.TrimEnd('/'); 

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
                $"{webGatewaySwaggerRootUrl}/swagger/oauth2-redirect.html", // WebGateway redirect uri
                $"{publicGatewaySwaggerRootUrl}/swagger/oauth2-redirect.html", // PublicGateway redirect uri
                $"{authServerRootUrl}/swagger/oauth2-redirect.html", // AuthServer redirect uri
                $"{identityServiceRootUrl}/swagger/oauth2-redirect.html", // IdentityService redirect uri
                $"{saasServiceServiceRootUrl}/swagger/oauth2-redirect.html", // SaasService redirect uri
                $"{auditLoggingServiceServiceRootUrl}/swagger/oauth2-redirect.html", // AuditLoggingService redirect uri
                $"{fileManagementServiceServiceRootUrl}/swagger/oauth2-redirect.html", // FileManagementService redirect uri
                $"{chatServiceServiceRootUrl}/swagger/oauth2-redirect.html", // ChatService redirect uri
                $"{administrationServiceRootUrl}/swagger/oauth2-redirect.html" // AdministrationService redirect uri
            },
            clientUri: webGatewaySwaggerRootUrl,
            logoUri: "/images/clients/swagger.svg"
        );
    }

    private async Task CreateScopesAsync(string name)
    {
        if (await _scopeRepository.FindByNameAsync(name) == null)
        {
            await _scopeManager.CreateAsync(new OpenIddictScopeDescriptor
            {
                Name = name,
                DisplayName = name + " API",
                Resources =
                {
                    name
                }
            });
        }
    }

    private async Task CreateClientsAsync()
    {
        var commonScopes = new List<string>
        {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        };

        //Blazor Client
        var blazorClientRootUrl = _configuration["OpenIddict:Applications:Blazor:RootUrl"].EnsureEndsWith('/');
        await CreateOrUpdateApplicationAsync(
            name: "Blazor",
            type: OpenIddictConstants.ClientTypes.Public,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: "Blazor Client",
            secret: null,
            grantTypes: new List<string> { OpenIddictConstants.GrantTypes.AuthorizationCode },
            scopes: commonScopes.Union(new[] { 
                "AuthServer",
                "IdentityService",
                "SaasService",
                "AuditLoggingService",
                "FileManagementService",
                "ChatService",
                "AdministrationService" }).ToList(),
            redirectUris: new List<string> { $"{blazorClientRootUrl}authentication/login-callback" },
            postLogoutRedirectUris: new List<string> { $"{blazorClientRootUrl}authentication/logout-callback" },
            clientUri: blazorClientRootUrl,
            logoUri: "/images/clients/blazor.svg"
        );

        
        //Web-Public Client
        var webPublicClientRootUrl = _configuration["OpenIddict:Applications:WebPublic:RootUrl"]!.EnsureEndsWith('/');
        await CreateOrUpdateApplicationAsync(
            name: "WebPublic",
            type: OpenIddictConstants.ClientTypes.Confidential,
            consentType: OpenIddictConstants.ConsentTypes.Implicit,
            displayName: "Public Web Client",
            secret: "1q2w3e*",
            grantTypes: new List<string> //Hybrid flow
            {
                OpenIddictConstants.GrantTypes.AuthorizationCode,
                OpenIddictConstants.GrantTypes.Implicit
            },
            scopes: commonScopes.Union(new[]
            {
                "AuthServer", 
                "AdministrationService"
            }).ToList(),
            redirectUris: new List<string> { $"{webPublicClientRootUrl}signin-oidc" },
            postLogoutRedirectUris: new List<string>() { $"{webPublicClientRootUrl}signout-callback-oidc" },
            clientUri: webPublicClientRootUrl,
            logoUri: "/images/clients/aspnetcore.svg"
        );
    }

    private async Task CreateOrUpdateApplicationAsync(
        string name,
        string type,
        string consentType,
        string displayName,
        string? secret,
        List<string> grantTypes,
        List<string> scopes,
        List<string>? redirectUris = null,
        List<string>? postLogoutRedirectUris = null,
        List<string>? permissions = null,
        string? clientUri = null,
        string? logoUri = null)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        
        if (!string.IsNullOrEmpty(secret) && string.Equals(type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
        {
            throw new ApplicationException("No client secret can be set for public applications");
        }

        if (string.IsNullOrEmpty(secret) && string.Equals(type, OpenIddictConstants.ClientTypes.Confidential, StringComparison.OrdinalIgnoreCase))
        {
            throw new ApplicationException("Client secret is required for confidential applications");
        }

        Check.NotNullOrEmpty(grantTypes, nameof(grantTypes));
        Check.NotNullOrEmpty(scopes, nameof(scopes));
        
        var application = new AbpApplicationDescriptor
        {
            ClientId = name,
            ClientType = type,
            ClientSecret = secret,
            ConsentType = consentType,
            DisplayName = displayName,
            ClientUri = clientUri,
            LogoUri = logoUri
        };

        if (new [] { OpenIddictConstants.GrantTypes.AuthorizationCode, OpenIddictConstants.GrantTypes.Implicit }.All(grantTypes.Contains))
        {
            application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);

            if (string.Equals(type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
            }
        }

        if (!redirectUris.IsNullOrEmpty() || !postLogoutRedirectUris.IsNullOrEmpty())
        {
            application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Logout);
        }

        var buildInGrantTypes = new []
        {
            OpenIddictConstants.GrantTypes.Implicit,
            OpenIddictConstants.GrantTypes.Password,
            OpenIddictConstants.GrantTypes.AuthorizationCode,
            OpenIddictConstants.GrantTypes.ClientCredentials,
            OpenIddictConstants.GrantTypes.DeviceCode,
            OpenIddictConstants.GrantTypes.RefreshToken
        };

        foreach (var grantType in grantTypes)
        {
            if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
            }

            if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode || grantType == OpenIddictConstants.GrantTypes.Implicit)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
            }

            if (grantType == OpenIddictConstants.GrantTypes.AuthorizationCode ||
                grantType == OpenIddictConstants.GrantTypes.ClientCredentials ||
                grantType == OpenIddictConstants.GrantTypes.Password ||
                grantType == OpenIddictConstants.GrantTypes.RefreshToken ||
                grantType == OpenIddictConstants.GrantTypes.DeviceCode)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Revocation);
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Introspection);
            }

            if (grantType == OpenIddictConstants.GrantTypes.ClientCredentials)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
            }

            if (grantType == OpenIddictConstants.GrantTypes.Implicit)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Implicit);
            }

            if (grantType == OpenIddictConstants.GrantTypes.Password)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
            }

            if (grantType == OpenIddictConstants.GrantTypes.RefreshToken)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
            }
            
            if (grantType == OpenIddictConstants.GrantTypes.DeviceCode)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
                application.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Device);
            }

            if (grantType == OpenIddictConstants.GrantTypes.Implicit)
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
                if (string.Equals(type, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
                {
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
                    application.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Token);
                }
            }

            if (!buildInGrantTypes.Contains(grantType))
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.GrantType + grantType);
            }
        }

        var buildInScopes = new []
        {
            OpenIddictConstants.Permissions.Scopes.Address,
            OpenIddictConstants.Permissions.Scopes.Email,
            OpenIddictConstants.Permissions.Scopes.Phone,
            OpenIddictConstants.Permissions.Scopes.Profile,
            OpenIddictConstants.Permissions.Scopes.Roles
        };

        foreach (var scope in scopes)
        {
            if (buildInScopes.Contains(scope))
            {
                application.Permissions.Add(scope);
            }
            else
            {
                application.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scope);
            }
        }

        if (redirectUris != null)
        {
            foreach (var redirectUri in redirectUris)
            {
                if (!Uri.TryCreate(redirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                {
                    throw new ApplicationException("Invalid redirect URI: " + redirectUri);
                }

                if (application.RedirectUris.All(x => x != uri))
                {
                    application.RedirectUris.Add(uri);
                }
            }
        }

        if (postLogoutRedirectUris != null)
        {
            foreach (var postLogoutRedirectUri in postLogoutRedirectUris)
            {
                if (!Uri.TryCreate(postLogoutRedirectUri, UriKind.Absolute, out var uri) || !uri.IsWellFormedOriginalString())
                {
                    throw new ApplicationException("Invalid post logout redirect URI:" + postLogoutRedirectUri);
                }

                if (application.PostLogoutRedirectUris.All(x => x != uri))
                {
                    application.PostLogoutRedirectUris.Add(uri);
                }
            }
        }

        if (permissions != null)
        {
            await _permissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                name,
                permissions
            );
        }
                
        var existingClient = await _applicationRepository.FindByClientIdAsync(name);
        if (existingClient != null)
        {
            await _applicationManager.UpdateAsync(existingClient.ToModel(), application);
        }
        else
        {
            await _applicationManager.CreateAsync(application);
        }
    }
}
