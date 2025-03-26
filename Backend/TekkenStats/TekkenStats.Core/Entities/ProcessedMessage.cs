namespace TekkenStats.Core.Entities;

public class ProcessedMessage
{
    public Guid Id { get; init; }= Guid.CreateVersion7();
    public Guid MessageId { get; init; }
}