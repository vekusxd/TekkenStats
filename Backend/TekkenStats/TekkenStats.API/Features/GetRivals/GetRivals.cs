using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using TekkenStats.API.Features.Shared;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.GetRivals;

public class GetRivals : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/rivals/{tekkenId}", Handler);
    }

    private async Task<Results<Ok<List<RivalsResponse>>, NotFound>> Handler(
        string tekkenId,
        [FromQuery] int? playerCharacterId,
        [FromQuery] int? opponentCharacterId,
        MongoDatabase mongoDatabase,
        CharacterStore characterStore)
    {
        var collection = mongoDatabase.Db.GetCollection<BsonDocument>(Player.CollectionName);
        var pipeline = new List<BsonDocument>
        {
            new("$match", new BsonDocument("_id", tekkenId)),
            new("$unwind", "$Matches")
        };

        var matchFilters = new BsonDocument();

        if (playerCharacterId.HasValue)
        {
            matchFilters.Add("Matches.Challenger.CharacterId", playerCharacterId.Value);
        }

        if (opponentCharacterId.HasValue)
        {
            matchFilters.Add("Matches.Opponent.CharacterId", opponentCharacterId.Value);
        }

        if (matchFilters.ElementCount > 0)
        {
            pipeline.Add(new BsonDocument("$match", matchFilters));
        }

        var groupStage = new BsonDocument
        {
            { "_id", "$Matches.Opponent.TekkenId" },
            { "OpponentName", new BsonDocument("$first", "$Matches.Opponent.Name") }, // Сохраняем имя оппонента
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
            { "TekkenId", "$_id" },
            { "Name", "$OpponentName" },
            { "TotalMatches", 1 },
            { "Wins", 1 },
            { "Losses", 1 }
        };
        pipeline.Add(new BsonDocument("$project", projectStage));

        pipeline.Add(new BsonDocument("$sort", new BsonDocument("TotalMatches", -1)));

        var cursor = await collection.AggregateAsync<PlayerOpponentStats>(pipeline);
        var result = await cursor.ToListAsync();

        if (result == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(result.Select(c => new RivalsResponse
        {
            TekkenId = c.TekkenId,
            Name = c.Name,
            Wins = c.Wins,
            Losses = c.Losses,
            TotalMatches = c.TotalMatches,
        }).ToList());
    }
}

public class PlayerOpponentStats
{
    public required string TekkenId { get; init; }
    public required string Name { get; init; }
    public int Wins { get; init; }
    public int Losses { get; init; }
    public int TotalMatches { get; init; }
}

public class RivalsResponse
{
    public required string TekkenId { get; init; }
    public required string Name { get; init; }
    public int TotalMatches { get; init; }
    public int Wins { get; init; }
    public int Losses { get; init; }
    public double WinRate => Math.Round((double)Wins / TotalMatches * 100, 2);
}