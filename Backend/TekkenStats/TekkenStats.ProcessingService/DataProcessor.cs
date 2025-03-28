using MassTransit;
using Microsoft.EntityFrameworkCore;
using TekkenStats.Core.Contracts;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.ProcessingService;

public class DataProcessor : IConsumer<DataMessage>
{
    private readonly ResponseProcessor _responseProcessor;
    private readonly ILogger<ResponseProcessor> _logger;
    private readonly AppDbContext _dbContext;

    public DataProcessor(ResponseProcessor responseProcessor,
        ILogger<ResponseProcessor> logger, AppDbContext dbContext)
    {
        _responseProcessor = responseProcessor;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<DataMessage> context)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var messageId = context.Message.MessageId;
            var message = await _dbContext.ProcessedMessages.FirstOrDefaultAsync(m => m.MessageId == messageId);
            if (message != null)
            {
                _logger.LogInformation("Message: {} already processed", messageId);
                await transaction.RollbackAsync();
                return;
            }

            _dbContext.ProcessedMessages.Add(new ProcessedMessage { MessageId = messageId });

            foreach (var item in context.Message.Responses)
            {
                await _responseProcessor.ProcessResponse(item);
            }

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("Response processing done");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error processing message");
        }
    }
}