using System.Net.Http.Json;
using MassTransit;
using TekkenStats.Core.Contracts;

namespace TekkenStats.CollectorService;

public class CollectorService : BackgroundService
{
    private readonly ILogger<CollectorService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CollectorService(ILogger<CollectorService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceProvider.CreateAsyncScope();

                var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

                var client = httpClientFactory.CreateClient("WavuWankClient");

                var result =
                    await client.GetFromJsonAsync<WavuWankResponse[]>("api/replays",
                        cancellationToken: stoppingToken) ?? throw new Exception("Response is null");

                _logger.LogInformation("Response count: {}", result.Length);

                var endpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var chunks = result.Chunk(500);

                foreach (var chunk in chunks)
                {
                    await endpoint.Publish(new DataMessage { MessageId = Guid.CreateVersion7(), Responses = chunk },
                        stoppingToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {ErrorMessage}", ex.Message);
            }
        }
    }
}