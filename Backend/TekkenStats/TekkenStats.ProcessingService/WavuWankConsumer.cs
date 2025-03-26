using System.Reflection.Metadata.Ecma335;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TekkenStats.Application.Contracts;
using TekkenStats.Application.Services;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.Seeder;

public class WavuWankConsumer : IConsumer<WavuWankMessage>
{
    private readonly WavuWankResponseProcessor _wavuWankResponseProcessor;
    private readonly ILogger<WavuWankResponseProcessor> _logger;
    private readonly AppDbContext _dbContext;

    public WavuWankConsumer(WavuWankResponseProcessor wavuWankResponseProcessor,
        ILogger<WavuWankResponseProcessor> logger,
        AppDbContext dbContext)
    {
        _wavuWankResponseProcessor = wavuWankResponseProcessor;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<WavuWankMessage> context)
    {
        var messageId = context.Message.MessageId;

        _logger.LogInformation("Handling message: {}", messageId);

        if (await IsProceeded(messageId))
        {
            _logger.LogInformation("Message: {} already handled", messageId);
            return;
        }

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await _dbContext.AddAsync(new ProcessedMessage { MessageId = messageId });
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            foreach (var item in context.Message.Responses)
            {
                await _wavuWankResponseProcessor.ProcessResponse(item);
            }

            _logger.LogInformation("{} battles processed", context.Message.Responses.Length);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (
            ex.InnerException is PostgresException { SqlState: "23505" })
        {
            _logger.LogError("Message handled: {MessageId}", messageId);
            await transaction.RollbackAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error processing message: {MessageId}, {}", messageId, ex.Message);
            await transaction.RollbackAsync();
        }
    }

    private async Task<bool> IsProceeded(Guid messageId)
    {
        return await _dbContext.ProcessedMessages.AnyAsync(m => m.MessageId == messageId);
    }
}