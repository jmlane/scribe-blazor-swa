using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

using Scribe.Shared;

using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;

namespace Scribe.Api;

public class Characters
{
    private readonly ILogger<Characters> _logger;
    private readonly Container _container;

    public Characters(ILogger<Characters> logger, CosmosClient cosmosClient)
    {
        _logger = logger;
        _container = cosmosClient.GetContainer("scribe", "characters");
    }

    [Function(nameof(NewCharacter))]
    public async Task<IActionResult> NewCharacter(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "characters")] HttpRequest req,
        [FromBody] Character character)
    {
        character.Id = Guid.NewGuid();
        var response = await _container.CreateItemAsync(character, new PartitionKey(character.Player));

        return new OkObjectResult(response);
    }

    [Function(nameof(GetCharacters))]
    public async Task<IActionResult> GetCharacters(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "characters")] HttpRequest req)
    {
        var query = new QueryDefinition("SELECT * FROM characters");
        var iterator = _container.GetItemQueryIterator<Character>(query);

        List<Character> characters = [];
        while (iterator.HasMoreResults)
        {
            FeedResponse<Character> result = await iterator.ReadNextAsync();
            characters.AddRange(result);
        }

        return new OkObjectResult(characters);
    }
}
