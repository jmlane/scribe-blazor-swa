using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.SignalRService;
using Microsoft.Extensions.Logging;

namespace Scribe.Api;

[SignalRConnection]
public class Chat(IServiceProvider serviceProvider, ILogger<Chat> logger) : ServerlessHub(serviceProvider)
{
    private const string HubName = nameof(Chat);

    [Function(nameof(Negotiate))]
    public async Task<HttpResponseData> Negotiate(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "chat/negotiate")] HttpRequestData req)
    {
        var negotiateResponse = await NegotiateAsync();
        var response = req.CreateResponse();
        await response.WriteBytesAsync(negotiateResponse.ToArray());
        return response;
    }

    [Function("OnConnected")]
    public Task OnConnected([SignalRTrigger(HubName, "connections", "connected")] SignalRInvocationContext context)
    {
        context.Headers.TryGetValue("Authorization", out var auth);
        logger.LogInformation($"{context.ConnectionId} has connected");
        return Clients.All.SendAsync("newConnection", new { context.ConnectionId, Authorization = auth });
    }
}