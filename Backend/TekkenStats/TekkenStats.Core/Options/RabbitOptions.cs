namespace TekkenStats.Core.Options;

public class RabbitOptions
{
    public const string Section = "RabbitMQOptions";
    public required string Host { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}