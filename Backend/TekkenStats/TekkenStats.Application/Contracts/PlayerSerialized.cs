namespace TekkenStats.Application.Contracts;

public class PlayerSerialized
{
    public required string Id { get; set; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
}