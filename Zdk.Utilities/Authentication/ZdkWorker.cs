using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Zdk.Utilities.Authentication;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Zdk.Utilities.Authentication;

public class ZdkWorker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    private string clientId;
    private string displayName;
    private HashSet<Uri> postLogoutRedirectUris;
    private HashSet<Uri> redirectUris;
    private HashSet<string> permissions;
    private HashSet<string> requirements;

    public ZdkWorker(IServiceProvider serviceProvider, string clientId, string displayName, HashSet<Uri> postLogoutRedirectUris = null, HashSet<Uri> redirectUris = null, HashSet<string> permissions = null, HashSet<string> requirements = null)
    {
        _serviceProvider = serviceProvider;

        if (postLogoutRedirectUris == null)
        {
            postLogoutRedirectUris = new HashSet<Uri>()
            {
                new Uri("https://www.zdk.no/authentication/logout-callback"),
                new Uri("https://zdk.no/authentication/logout-callback"),
                new Uri("https://localhost:44367/authentication/logout-callback"),
            };
        }

        if (redirectUris == null)
        {
            redirectUris = new HashSet<Uri>()
            {
                new Uri("https://www.zdk.no/authentication/login-callback"),
                new Uri("https://zdk.no/authentication/login-callback"),
                new Uri("https://localhost:44367/authentication/login-callback"),
            };
        }

        if (permissions == null)
        {
            permissions = new HashSet<string>()
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Logout,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
            };
        }

        if (requirements == null)
        {
            requirements = new HashSet<string>()
            {
                Requirements.Features.ProofKeyForCodeExchange,
            };
        }

        this.clientId = clientId;
        this.displayName = displayName;
        this.postLogoutRedirectUris = postLogoutRedirectUris;
        this.redirectUris = redirectUris;
        this.permissions = permissions;
        this.requirements = requirements;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<AuthContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync(clientId, cancellationToken) is null)
        {
            OpenIddictApplicationDescriptor openIddictApplicationDescriptor = new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ConsentType = ConsentTypes.Explicit,
                DisplayName = displayName,
                Type = ClientTypes.Public,
            };

            openIddictApplicationDescriptor.PostLogoutRedirectUris.UnionWith(postLogoutRedirectUris);
            openIddictApplicationDescriptor.RedirectUris.UnionWith(redirectUris);
            openIddictApplicationDescriptor.Permissions.UnionWith(permissions);
            openIddictApplicationDescriptor.Requirements.UnionWith(requirements);

            await manager.CreateAsync(openIddictApplicationDescriptor, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
