using TekkenStats.Core.Entities;

namespace TekkenStats.Application.Contracts;

public class WavuWankMessage
{
    public Guid MessageId { get; set; }
    public required  WavuWankResponse[] Responses { get; init; }
}