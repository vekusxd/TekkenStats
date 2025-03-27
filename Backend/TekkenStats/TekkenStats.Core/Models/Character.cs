namespace TekkenStats.Core.Models;

public class Character
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Abbreviation { get; set; }
    public ICollection<CharacterInfo> CharacterInfos { get; set; } = [];
    public ICollection<Battle> Character1Battles { get; set; } = [];
    public ICollection<Battle> Character2Battles { get; set; } = [];
}