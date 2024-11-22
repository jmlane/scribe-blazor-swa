using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.SignalRService;
using Microsoft.Extensions.Logging;

using Scribe.Shared.Chat;

namespace Scribe.Api;

[SignalRConnection]
public class Chat(IServiceProvider serviceProvider, ILogger<Chat> logger) : ServerlessHub<IChatClient>(serviceProvider)
{
    private const string HubName = nameof(Chat);

    [Function(nameof(Negotiate))]
    public async Task<HttpResponseData> Negotiate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chat/negotiate")] HttpRequestData req)
    {
        ClaimsPrincipal identity = GetClaimsPrincipal(req);
        string? userId = identity.FindFirstValue(ClaimTypes.Name);
 
        var negotiateResponse = await NegotiateAsync(new() { UserId = userId });
        var response = req.CreateResponse();
        await response.WriteBytesAsync(negotiateResponse.ToArray());
        return response;
    }

    [Function(nameof(OnConnected))]
    public void OnConnected([SignalRTrigger(HubName, "connections", "connected")] SignalRInvocationContext context)
    {
        logger.LogInformation($"{context.UserId} ({context.ConnectionId}) has connected");
    }

    [Function(nameof(SendMessage))]
    public Task SendMessage([SignalRTrigger(HubName, "messages", "sendMessage", "message")] SignalRInvocationContext context, string message)
    {
        return Clients.All.NewMessage(new(context.UserId, message));
    }

    private static ClaimsPrincipal GetClaimsPrincipal(HttpRequestData req)
    {
        ClientPrincipal? principal = null;

        if (req.Headers.TryGetValues("x-ms-client-principal", out var headers))
        {
            var data = headers.First();
            var decoded = Convert.FromBase64String(data);
            var json = Encoding.UTF8.GetString(decoded);
            principal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        if (principal is null)
        {
            return new ClaimsPrincipal();
        }

        var identity = new ClaimsIdentity(principal.IdentityProvider);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
        identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails));
        identity.AddClaims(principal.UserRoles.Select(r => new Claim(ClaimTypes.Role, r)));

        return new ClaimsPrincipal(identity);
    }

    private class ClientPrincipal
    {
        public string IdentityProvider { get; init; }
        public string UserId { get; init; }
        public string UserDetails { get; init; }
        public IEnumerable<string> UserRoles { get; init; }
    }
}