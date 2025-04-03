using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using TekkenStats.API.Features.Shared;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.GetRivals;

public class GetRivalsRequest
{
    [FromQuery] public int? PlayerCharacterId { get; init; }
    [FromQuery] public int? OpponentCharacterId { get; init; }
    [FromQuery] public int? PageSize { get; set; }
    [FromQuery] public int? PageNumber { get; set; }
}

public class GetRivalsResponse
{
    public required List<Rival> Data { get; init; }
    public int Count { get; init; }
}

public class GetRivals : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/rivals/{tekkenId}", Handler);
    }

    private async Task<Results<Ok<GetRivalsResponse>, NotFound, ValidationProblem>> Handler(
        string tekkenId,
        [AsParameters] GetRivalsRequest request,
        IValidator<GetRivalsRequest> validator,
        MongoDatabase db,
        CharacterStore characterStore)
    {
        request.PageNumber ??= 1;
        request.PageSize ??= 10;
        var skip = (request.PageNumber - 1) * request.PageSize;

        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());


        var collection = db.Players;
        
        var pipeline = new List<BsonDocument>
        {
            new("$match", new BsonDocument("_id", tekkenId)),
            new("$unwind", "$Matches")
        };

        var matchFilters = new BsonDocument();

        if (request.PlayerCharacterId.HasValue)
        {
            matchFilters.Add("Matches.Challenger.CharacterId", request.PlayerCharacterId.Value);
        }

        if (request.OpponentCharacterId.HasValue)
        {
            matchFilters.Add("Matches.Opponent.CharacterId", request.OpponentCharacterId.Value);
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

        pipeline.Add(new BsonDocument("$sort", new BsonDocument("TotalMatches", -1)));

        var countPipeline = new List<BsonDocument>(pipeline);
        countPipeline.Add(new BsonDocument("$count", "totalCount"));

        var countCursor = await collection.AggregateAsync<BsonDocument>(countPipeline);
        var countResult = await countCursor.FirstOrDefaultAsync();
        var totalCount = countResult?["totalCount"].AsInt32 ?? 0;

        pipeline.Add(new BsonDocument("$skip", skip));
        pipeline.Add(new BsonDocument("$limit", request.PageSize));

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


        var cursor = await collection.AggregateAsync<PlayerOpponentStats>(pipeline);
        var result = await cursor.ToListAsync();

        if (result == null)
            return TypedResults.NotFound();

        var data = result
            .Select(c => c
                .ToRival())
            .ToList();

        return TypedResults.Ok(new GetRivalsResponse
        {
            Data = data,
            Count = totalCount
        });
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

public class Rival
{
    public required string TekkenId { get; init; }
    public required string Name { get; init; }
    public int TotalMatches { get; init; }
    public int Wins { get; init; }
    public int Losses { get; init; }
    public double WinRate => Math.Round((double)Wins / TotalMatches * 100, 2);
}