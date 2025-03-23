using System.Net.Http.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TekkenStats.Core.Models;

namespace TekkenStats.Application.Services;

public class WavuWankProducer : BackgroundService
{
    private readonly ILogger<WavuWankProducer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public WavuWankProducer(ILogger<WavuWankProducer> logger, IServiceProvider serviceProvider)
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

                _logger.LogError("Response count: {}", result.Length);

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                await publishEndpoint.Publish(result, stoppingToken);

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occured: {ErrorMessage}", ex.Message);
            }
        }
    }
}