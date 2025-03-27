using MongoDB.Driver;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;
using Player = TekkenStats.Core.Entities.Player;

namespace TekkenStats.Seeder;

public class ResponseProcessor
{
    private readonly ILogger<ResponseProcessor> _logger;
    private readonly IMongoDatabase _db;

    public ResponseProcessor(ILogger<ResponseProcessor> logger, MongoDatabase database)
    {
        _logger = logger;
        _db = database.Db;
    }

    public async Task ProcessResponse(WavuWankResponse response)
    {
        var date = DateTimeOffset.FromUnixTimeMilliseconds(response.BattleAt).DateTime;

        var firstPlayerInfo = new PlayerInfo(
            characterId: response.P1CharaId,
            ratingBefore: response.P1RatingBefore ?? 0,
            ratingChange: response.P1RatingChange ?? 0,
            rounds: response.P1Rounds);

        var secondPlayerInfo = new PlayerInfo(
            characterId: response.P2CharaId,
            ratingBefore: response.P2RatingBefore ?? 0,
            ratingChange: response.P2RatingChange ?? 0,
            rounds: response.P2Rounds);

        var firstPlayerMatch = new Match
        {
            BattleId = response.BattleId,
            Date = date,
            GameVersion = response.GameVersion,
            Winner = response.Winner == 1,
            Challenger = firstPlayerInfo,
            Opponent = new OpponentInfo(secondPlayerInfo, response.P2PolarisId)
        };

        var secondPlayerMatch = new Match
        {
            BattleId = response.BattleId,
            Date = date,
            GameVersion = response.GameVersion,
            Winner = response.Winner == 2,
            Challenger = secondPlayerInfo,
            Opponent = new OpponentInfo(firstPlayerInfo, response.P1PolarisId)
        };

        var firstPlayerUpdate = UpdatePlayer(new PlayerUpdate
        {
            TekkenId = response.P1PolarisId,
            Power = response.P1Power,
            Rank = response.P1Rank,
            Name = response.P1Name,
            Match = firstPlayerMatch
        });

        var secondPlayerUpdate = UpdatePlayer(new PlayerUpdate
        {
            TekkenId = response.P2PolarisId,
            Power = response.P2Power,
            Rank = response.P2Rank,
            Name = response.P2Name,
            Match = secondPlayerMatch
        });

        await Task.WhenAll(firstPlayerUpdate, secondPlayerUpdate);
    }

    private async Task UpdatePlayer(PlayerUpdate player)
    {
        var collection = _db.GetCollection<Player>(Player.CollectionName);
        var filter = Builders<Player>.Filter.Eq(p => p.TekkenId, player.TekkenId);

        var update = Builders<Player>.Update
            .Set(p => p.Power, player.Power)
            .Set(p => p.Rank, player.Rank)
            .AddToSet(p => p.Names, player.Name)
            .Push(p => p.Matches, player.Match);

        await collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    private class PlayerUpdate
    {
        public required string TekkenId { get; init; }
        public required int Power { get; init; }
        public required int Rank { get; init; }
        public required string Name { get; init; }
        public required Match Match { get; init; }
    }
}