using MassTransit;
using Microsoft.Extensions.Logging;
using TekkenStats.Core.Models;

namespace TekkenStats.Application.Services;

public class WavuWankConsumer : IConsumer<WavuWankResponse[]>
{
    private readonly WavuWankResponseProcessor _wavuWankResponseProcessor;
    private readonly ILogger<WavuWankResponseProcessor> _logger;

    public WavuWankConsumer(WavuWankResponseProcessor wavuWankResponseProcessor, ILogger<WavuWankResponseProcessor> logger)
    {
        _wavuWankResponseProcessor = wavuWankResponseProcessor;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<WavuWankResponse[]> context)
    {
        var counter = 0;
        foreach (var item in context.Message)
        {
            // await _responseProcessor.ProcessResponse(item);
            _logger.LogInformation("Processing request from consumer: {counter}", counter++);
        }
    }
}