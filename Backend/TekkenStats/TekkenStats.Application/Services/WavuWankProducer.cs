﻿using System.Net.Http.Json;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TekkenStats.Application.Contracts;
using TekkenStats.Core.Entities;

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

                _logger.LogInformation("Response count: {}", result.Length);

                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                var chunks = result.Chunk(500);

                foreach (var chunk in chunks)
                {
                    await publishEndpoint.Publish(new WavuWankMessage { MessageId = Guid.NewGuid(), Responses = chunk },
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