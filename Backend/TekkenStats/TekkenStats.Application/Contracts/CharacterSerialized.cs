namespace TekkenStats.Application.Contracts;

public class CharacterSerialized
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Abbreviation { get; init; }
}