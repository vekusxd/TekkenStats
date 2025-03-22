using System.Net.Http.Json;
using TekkenStats.Core.Models;
using TekkenStats.DataAccess;

namespace TekkenStats.Seeder;

public class Seeder : BackgroundService
{
    private readonly ILogger<Seeder> _logger;
    private readonly IServiceProvider _serviceProvider;
    private static int _beginTime = 1742680788 + 700;
    private static int _counter = 1;

    public Seeder(ILogger<Seeder> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_beginTime <= 0)
            {
                _logger.LogInformation($"Seeder done, {_counter} requests completed in total");
                return;
            }

            await using var scope = _serviceProvider.CreateAsyncScope();
            var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();

            var client = httpClientFactory.CreateClient("WavuWankClient");

            _beginTime -= 700;

            var result =
                client.GetFromJsonAsAsyncEnumerable<WavuWankResponse>($"api/replays?before={_beginTime}",
                    cancellationToken: stoppingToken);

            _logger.LogInformation($"Request: {_counter++}");

            await foreach (var item in result)
            {
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}