namespace TekkenStats.Core.Entities;

public class ProcessedMessage
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required Guid MessageId { get; set; } 
}