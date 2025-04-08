using Elastic.Clients.Elasticsearch;
using MongoDB.Driver;
using TekkenStats.Core.Contracts;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;
using TekkenStats.DataAccess.Models;
using Name = TekkenStats.Core.Entities.Name;
using Player = TekkenStats.Core.Entities.Player;

namespace TekkenStats.ProcessingService;

public class ResponseProcessor
{
    private readonly ElasticSearch _elasticSearch;
    private readonly IMongoDatabase _db;

    public ResponseProcessor(MongoDatabase database, ElasticSearch elasticSearch)
    {
        _elasticSearch = elasticSearch;
        _db = database.Db;
    }

    public async Task ProcessResponse(WavuWankResponse response)
    {
        var date = DateTimeOffset.FromUnixTimeSeconds(response.BattleAt).DateTime;

        var firstPlayerInfo = new ChallengerInfo
        {
            CharacterId = response.P1CharaId,
            RatingBefore = response.P1RatingBefore ?? 0,
            RatingChange = response.P1RatingChange ?? 0,
            Rounds = response.P1Rounds,
            Name = response.P1Name,
            TekkenId = response.P1PolarisId
        };

        var secondPlayerInfo = new ChallengerInfo
        {
            CharacterId = response.P2CharaId,
            RatingBefore = response.P2RatingBefore ?? 0,
            RatingChange = response.P2RatingChange ?? 0,
            Rounds = response.P2Rounds,
            Name = response.P2Name,
            TekkenId = response.P2PolarisId
        };

        var firstPlayerMatch = new Match
        {
            BattleId = response.BattleId,
            Date = date,
            GameVersion = response.GameVersion,
            Winner = response.Winner == 1,
            Challenger = firstPlayerInfo,
            Opponent = secondPlayerInfo,
        };

        var secondPlayerMatch = new Match
        {
            BattleId = response.BattleId,
            Date = date,
            GameVersion = response.GameVersion,
            Winner = response.Winner == 2,
            Challenger = secondPlayerInfo,
            Opponent = firstPlayerInfo
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

        var firstPlayerNameIndex = _elasticSearch.IndexPlayer(response.P1PolarisId, response.P1Name);
        var secondPlayerNameIndex = _elasticSearch.IndexPlayer(response.P2PolarisId, response.P2Name);

        await Task.WhenAll(firstPlayerUpdate, secondPlayerUpdate, firstPlayerNameIndex, secondPlayerNameIndex);
    }

    private async Task UpdatePlayer(PlayerUpdate player)
    {
        var collection = _db.GetCollection<Player>(Player.CollectionName);
        var filter = Builders<Player>.Filter.Eq(p => p.TekkenId, player.TekkenId);

        var existingPlayer = await collection.Find(filter).FirstOrDefaultAsync();

        var battleIdExistsFilter = Builders<Player>.Filter.And(
            filter,
            Builders<Player>.Filter.ElemMatch(
                p => p.Matches,
                m => m.BattleId == player.Match.BattleId)
        );

        var battleIdExists = await collection.Find(battleIdExistsFilter).AnyAsync();

        if (battleIdExists) return;

        var update = Builders<Player>.Update
            .Set(p => p.Power, player.Power)
            .Set(p => p.Rank, player.Rank)
            .Push(p => p.Matches, player.Match);

        if (existingPlayer == null || existingPlayer.CurrentName != player.Name)
        {
            update = update
                .Set(p => p.CurrentName, player.Name)
                .Push(p => p.Names, new Name { PlayerName = player.Name, Date = DateTime.UtcNow });
        }

        var characterExists =
            existingPlayer?.Characters?.Any(c => c.CharacterId == player.Match.Challenger.CharacterId) ?? false;

        if (characterExists)
        {
            var characterFilter = filter & Builders<Player>.Filter.ElemMatch(
                p => p.Characters,
                c => c.CharacterId == player.Match.Challenger.CharacterId
            );
            var characterUpdate = Builders<Player>.Update
                .Inc("Characters.$.MatchesCount", 1)
                .Inc("Characters.$.WinCount", player.Match.Winner ? 1 : 0)
                .Inc("Characters.$.LossCount", player.Match.Winner ? 0 : 1)
                .Set("Characters.$.LastPlayed", player.Match.Date);


            if (player.Match.Challenger.RatingBefore != 0)
            {
                characterUpdate = characterUpdate.Set("Characters.$.Rating",
                    player.Match.Challenger.RatingBefore +
                    player.Match.Challenger.RatingChange);
            }

            await collection.UpdateOneAsync(characterFilter, characterUpdate);
        }
        else
        {
            var characterInfo = new CharacterInfo
            {
                CharacterId = player.Match.Challenger.CharacterId,
                MatchesCount = 1,
                WinCount = player.Match.Winner ? 1 : 0,
                LossCount = player.Match.Winner ? 0 : 1,
                Rating = player.Match.Challenger.RatingBefore + player.Match.Challenger.RatingChange,
                LastPlayed = player.Match.Date
            };
            update = update.Push(p => p.Characters, characterInfo);
        }

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