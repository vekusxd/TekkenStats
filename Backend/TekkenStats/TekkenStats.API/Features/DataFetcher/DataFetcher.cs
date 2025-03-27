using MassTransit;
using TekkenStats.Core.Entities;

namespace TekkenStats.API.Features.DataFetcher;

public class DataFetcher : BackgroundService
{
    private readonly ILogger<DataFetcher> _logger;
    private readonly IServiceProvider _serviceProvider;

    public DataFetcher(ILogger<DataFetcher> logger, IServiceProvider serviceProvider)
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

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                
                //TODO
                // var chunks = result.Chunk(500);
                //
                // foreach (var chunk in chunks)
                // {
                //     await publishEndpoint.Publish(
                //         new DataMessage { MessageId = Guid.CreateVersion7(), Responses = chunk }, stoppingToken);
                // }

                await publishEndpoint.Publish(new DataMessage { MessageId = Guid.CreateVersion7(), Responses = result },
                    stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {ErrorMessage}", ex.Message);
            }
        }
    }
}