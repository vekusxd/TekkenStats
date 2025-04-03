using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using TekkenStats.API.Features.Shared;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.GetMatchups;

public class MatchupsResponse
{
    public int OpponentCharacterId { get; init; }
    public required string CharacterName { get; init; }
    public required string CharacterImgURL { get; init; }
    public int TotalMatches { get; init; }
    public int Wins { get; init; }
    public int Losses { get; init; }
    public double WinRate => Math.Round((double)Wins / TotalMatches * 100, 2);
}

public class GetMatchups : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/matchups/{tekkenId}", Handler);
    }

    private async Task<Results<Ok<List<MatchupsResponse>>, NotFound>> Handler(
        string tekkenId,
        [FromQuery] int? playerCharacterId,
        MongoDatabase db,
        CharacterStore characterStore)
    {
        var collection = db.Players;
        
        var pipeline = new List<BsonDocument>
        {
            new BsonDocument("$match", new BsonDocument("_id", tekkenId)),
            new BsonDocument("$unwind", "$Matches")
        };

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

        return TypedResults.Ok(result
            .Select(c => c
                .ToMatchupResponse(characterStore))
            .ToList());
    }
}

public class PlayerMatchStats
{
    public int OpponentCharacterId { get; init; }
    public int TotalMatches { get; init; }
    public int Wins { get; init; }
    public int Losses { get; init; }
}