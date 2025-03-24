using MassTransit;
using TekkenStats.Application.Services;
using TekkenStats.Core.Models;

namespace TekkenStats.Seeder;

public class WavuWankConsumer : IConsumer<WavuWankResponse[]>
{
    private readonly WavuWankResponseProcessor _wavuWankResponseProcessor;
    private readonly ILogger<WavuWankResponseProcessor> _logger;

    public WavuWankConsumer(WavuWankResponseProcessor wavuWankResponseProcessor,
        ILogger<WavuWankResponseProcessor> logger)
    {
        _wavuWankResponseProcessor = wavuWankResponseProcessor;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<WavuWankResponse[]> context)
    {
        foreach (var item in context.Message)
        {
            await _wavuWankResponseProcessor.ProcessResponse(item);
        }
        _logger.LogInformation("Response processing done");
    }
}