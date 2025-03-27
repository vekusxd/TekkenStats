namespace TekkenStats.Core.Entities;

public class DataMessage
{
    public Guid MessageId { get; set; }
    public WavuWankResponse[] Responses { get; set; } = [];
}