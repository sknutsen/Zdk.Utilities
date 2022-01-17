using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Zdk.Utilities.Authentication.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Zdk.Utilities.Authentication;

public abstract class Worker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    protected OpenIddictApplicationDescriptor openIddictApplicationDescriptor;
    protected string clientId;
    protected string displayName;

    public Worker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        SetValues();
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<AuthContext>();
        await context.Database.EnsureCreatedAsync(cancellationToken);

        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        if (await manager.FindByClientIdAsync(clientId, cancellationToken) is null)
        {
            SetDescriptor();
            await manager.CreateAsync(openIddictApplicationDescriptor, cancellationToken);
        }

    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    protected virtual void SetDescriptor()
    {
        openIddictApplicationDescriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = clientId,
            ConsentType = ConsentTypes.Explicit,
            DisplayName = displayName,
            Type = ClientTypes.Public,
            PostLogoutRedirectUris =
            {
                new Uri("https://www.zdk.no/authentication/logout-callback"),
                new Uri("https://zdk.no/authentication/logout-callback"),
                new Uri("https://localhost:44367/authentication/logout-callback"),
            },
            RedirectUris =
            {
                new Uri("https://www.zdk.no/authentication/login-callback"),
                new Uri("https://zdk.no/authentication/login-callback"),
                new Uri("https://localhost:44367/authentication/login-callback"),
            },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Logout,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }
        };
    }

    protected virtual void SetValues()
    {
        clientId = "";
        displayName = "";
    }
}
