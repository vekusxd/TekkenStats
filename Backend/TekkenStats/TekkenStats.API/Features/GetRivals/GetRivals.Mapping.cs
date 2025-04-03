namespace TekkenStats.API.Features.GetRivals;

public static class PlayerOpponentStatsExtension
{
    public static Rival ToRival(this PlayerOpponentStats playerOpponentStats)
    {
        return new Rival
        {
            TekkenId = playerOpponentStats.TekkenId,
            Name = playerOpponentStats.Name,
            Wins = playerOpponentStats.Wins,
            Losses = playerOpponentStats.Losses,
            TotalMatches = playerOpponentStats.TotalMatches,
        };
    }
}