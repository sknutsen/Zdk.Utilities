using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Zdk.Utilities.Authentication.Data;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace Zdk.Utilities.Authentication;

public static class ProgramSetup
{
    public static IServiceCollection SetupZdkAuthFull(this IServiceCollection services, Action<DbContextOptionsBuilder> dbOptions, X509Certificate2 certificate, string clientId, string displayName)
    {
        services.SetupZdkDb(dbOptions);
        services.SetupZdkIdentity();
        services.SetupZdkOpenIddict(certificate);

        services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

        services.ConfigureZdkAuth();

        services.AddZdkWorker(clientId, displayName);

        return services;
    }

    public static IServiceCollection SetupZdkDb(this IServiceCollection services, Action<DbContextOptionsBuilder> dbOptions)
    {
        return services.AddDbContext<AuthContext>(options =>
        {
            dbOptions(options);
            options.UseOpenIddict();
        });
    }

    public static IdentityBuilder SetupZdkIdentity(this IServiceCollection services)
    {
        return services.AddDefaultIdentity<ZdkUser>(options => options.SignIn.RequireConfirmedAccount = false)
                       .AddEntityFrameworkStores<AuthContext>()
                       .AddDefaultTokenProviders();
    }

    public static OpenIddictBuilder SetupZdkOpenIddict(this IServiceCollection services, X509Certificate2 certificate)
    {
        return services.AddOpenIddict()
                       .AddCore(options =>
                       {
                           options.UseEntityFrameworkCore().UseDbContext<AuthContext>();
                       })
                       .AddServer(options =>
                       {
                           // Enable the authorization, logout, token and userinfo endpoints.
                           options.SetAuthorizationEndpointUris("/connect/authorize")
                                  .SetLogoutEndpointUris("/connect/logout")
                                  .SetTokenEndpointUris("/connect/token")
                                  .SetUserinfoEndpointUris("/connect/userinfo");

                           // Mark the "email", "profile" and "roles" scopes as supported scopes.
                           options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

                           // Note: the sample uses the code and refresh token flows but you can enable
                           // the other flows if you need to support implicit, password or client credentials.
                           options.AllowAuthorizationCodeFlow()
                                  .AllowRefreshTokenFlow();

                           // Register the signing and encryption credentials.
                           options.AddEncryptionCertificate(certificate)
                                  .AddSigningCertificate(certificate);

                           // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                           options.UseAspNetCore()
                                  .EnableAuthorizationEndpointPassthrough()
                                  .EnableLogoutEndpointPassthrough()
                                  .EnableStatusCodePagesIntegration()
                                  .EnableTokenEndpointPassthrough();
                       })
                       .AddValidation(options =>
                       {
                           options.UseLocalServer();
                           options.UseAspNetCore();
                       });
    }

    public static IServiceCollection ConfigureZdkAuth(this IServiceCollection services)
    {
        return services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserNameClaimType = Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = Claims.Role;

            // Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // User settings.
            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = false;
        });
    }

    public static IServiceCollection AddZdkWorker(this IServiceCollection services, string clientId, string displayName)
    {
        return services.AddHostedService<ZdkWorker>(e => new ZdkWorker(e, clientId, displayName));
    }
}
