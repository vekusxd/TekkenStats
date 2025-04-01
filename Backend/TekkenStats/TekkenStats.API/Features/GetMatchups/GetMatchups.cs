using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using TekkenStats.API.Features.Shared;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.GetMatchups;

public class GetMatchups : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/matchups/{tekkenId}", Handler);
    }

    private async Task<Results<Ok<List<MatchupsResponse>>, NotFound>> Handler(
        string tekkenId,
        [FromQuery] int? playerCharacterId,
        MongoDatabase mongoDatabase,
        CharacterStore characterStore)
    {
        var collection = mongoDatabase.Db.GetCollection<BsonDocument>(Player.CollectionName);
        var pipeline = new List<BsonDocument>();

        pipeline.Add(new BsonDocument("$match", new BsonDocument("_id", tekkenId)));

        pipeline.Add(new BsonDocument("$unwind", "$Matches"));

        if (playerCharacterId.HasValue)
        {
            pipeline.Add(new BsonDocument("$match",
                new BsonDocument("Matches.Challenger.CharacterId", playerCharacterId.Value)));
        }

        var groupStage = new BsonDocument
        {
            { "_id", "$Matches.Opponent.CharacterId" },
            { "Wins", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray { "$Matches.Winner", 1, 0 })) },
            {
                "Losses", new BsonDocument("$sum", new BsonDocument("$cond", new BsonArray { "$Matches.Winner", 0, 1 }))
            },
            { "TotalMatches", new BsonDocument("$sum", 1) }
        };
        pipeline.Add(new BsonDocument("$group", groupStage));

        var projectStage = new BsonDocument
        {
            { "_id", 0 },
            { "OpponentCharacterId", "$_id" },
            { "TotalMatches", 1 },
            { "Wins", 1 },
            { "Losses", 1 }
        };
        pipeline.Add(new BsonDocument("$project", projectStage));

        pipeline.Add(new BsonDocument("$sort", new BsonDocument("TotalMatches", -1)));

        var cursor = await collection.AggregateAsync<PlayerMatchStats>(pipeline);

        var result = await cursor.ToListAsync();

        if (result == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(result.Select(c => new MatchupsResponse
        {
            OpponentCharacterId = c.OpponentCharacterId,
            Wins = c.Wins,
            Losses = c.Losses,
            TotalMatches = c.TotalMatches,
            CharacterName = characterStore.GetCharacter(c.OpponentCharacterId).Name
        }).ToList());
    }
}

public class PlayerMatchStats
{
    public int OpponentCharacterId { get; init; }
    public int TotalMatches { get; init; }
    public int Wins { get; init; }
    public int Losses { get; init; }
}

public class MatchupsResponse
{
    public int OpponentCharacterId { get; init; }
    public required string CharacterName { get; init; }
    public int TotalMatches { get; init; }
    public int Wins { get; init; }
    public int Losses { get; init; }
    public double WinRate => Math.Round((double)Wins / TotalMatches, 2) * 100;
}