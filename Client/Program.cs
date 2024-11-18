using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Scribe.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string apiUrl = new((builder.Configuration["API_Prefix"] ?? builder.HostEnvironment.BaseAddress) + "/api/");
builder.Services.AddSingleton(new ChatConfig(new Uri(apiUrl + "chat")));
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new(apiUrl) });

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, StaticWebAppsAuthenticationStateProvider>();

await builder.Build().RunAsync();
