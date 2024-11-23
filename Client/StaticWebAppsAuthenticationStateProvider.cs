using System.Net.Http.Json;
using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using Scribe.Shared;

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

            return principal is null
                ? new AuthenticationState(new ClaimsPrincipal())
                : new(new(principal.ToClaimsIdentity()));
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
