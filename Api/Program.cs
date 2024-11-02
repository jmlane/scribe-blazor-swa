using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((hostContext ,services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddAzureClients(clientBuilder =>
        {
            clientBuilder.AddClient<CosmosClient, CosmosClientOptions>(
                options =>
                {
                    options.UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    };

                    return new CosmosClient(
                        hostContext.Configuration["CosmosDbConnectionString"],
                        options);
                });
                    
        });
    }) 
    .Build();

host.Run();
