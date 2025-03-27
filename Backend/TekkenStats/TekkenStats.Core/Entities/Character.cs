namespace TekkenStats.Core.Entities;

public class Character
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Abbreviation { get; set; }
}