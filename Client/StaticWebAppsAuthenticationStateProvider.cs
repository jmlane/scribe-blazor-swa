using System.Net.Http.Json;
using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Scribe.Client;

public class StaticWebAppsAuthenticationStateProvider(
    ILogger<StaticWebAppsAuthenticationStateProvider> logger,
    IWebAssemblyHostEnvironment environment)
    : AuthenticationStateProvider
{
    public async override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            HttpClient http = new() { BaseAddress = new Uri(environment.BaseAddress) };
            AuthenticationData? data = await http.GetFromJsonAsync<AuthenticationData>("/.auth/me");

            ClientPrincipal? principal = data?.ClientPrincipal;

            if (principal is null || principal.UserRoles is null)
            {
                return new(new ClaimsPrincipal());
            }

            ClaimsIdentity identity = new(principal.IdentityProvider);
            identity.AddClaim(new(ClaimTypes.NameIdentifier, principal.UserId!));
            identity.AddClaim(new(ClaimTypes.Name, principal.UserDetails!));
            identity.AddClaims(principal.UserRoles!.Select(role => new Claim(ClaimTypes.Role, role)));

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to retrieve authentication data");
            return new(new ClaimsPrincipal());
        }
    }
}

class AuthenticationData
{
    public ClientPrincipal? ClientPrincipal { get; init; }
}

class ClientPrincipal
{
    public string? IdentityProvider { get; init; }
    public string? UserId { get; init; }
    public string? UserDetails { get; init;}
    public List<string>? UserRoles { get; init; }
}