namespace TekkenStats.Core.Entities;

public class PlayerInfo
{
    public PlayerInfo(int characterId, int ratingBefore, int ratingChange, int rounds)
    {
        CharacterId = characterId;
        RatingBefore = ratingBefore;
        RatingChange = ratingChange;
        Rounds = rounds;
    }

    public int CharacterId { get; set; }
    public int Rounds { get; set; }
    public int RatingBefore { get; set; }
    public int RatingChange { get; set; }
}