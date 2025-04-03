using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using TekkenStats.API.Features.Shared;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.GetMatchHistory;

public class GetMatchHistoryRequest
{
    [FromQuery] public int? PageSize { get; set; } = 10;
    [FromQuery] public int? PageNumber { get; set; } = 1;
    [FromQuery] public int? CharacterId { get; init; }
    [FromQuery] public int? OpponentCharacterId { get; init; }
}

public class GetMatchHistoryResponse
{
    public List<MatchResponse> Matches { get; set; } = [];
    public int TotalMatches { get; set; }
}


public class GetMatchHistory : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/matches/{tekkenId}/{opponentTekkenId?}", Handler);
    }

    private async Task<Results<Ok<GetMatchHistoryResponse>, NotFound, ValidationProblem>> Handler(
        string tekkenId,
        string? opponentTekkenId,
        [AsParameters] GetMatchHistoryRequest request,
        IValidator<GetMatchHistoryRequest> validator,
        MongoDatabase db,
        CharacterStore characterStore)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var collection = db.Players;

        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;
        var skip = (pageNumber - 1) * pageSize;

        var pipeline = collection.Aggregate()
            .Match(p => p.TekkenId == tekkenId);

        var matchesFilter = BuildMatchesFilter(request.CharacterId, request.OpponentCharacterId, opponentTekkenId);

        var player = await pipeline
            .Project<PlayerMatchesProjection>(new BsonDocument
            {
                { "_id", 0 },
                {
                    "Matches", new BsonDocument
                    {
                        {
                            "$slice", new BsonArray
                            {
                                new BsonDocument
                                {
                                    {
                                        "$sortArray", new BsonDocument
                                        {
                                            {
                                                "input",
                                                matchesFilter != null
                                                    ? new BsonDocument
                                                    {
                                                        {
                                                            "$filter", new BsonDocument
                                                            {
                                                                { "input", "$Matches" },
                                                                { "as", "match" },
                                                                { "cond", matchesFilter }
                                                            }
                                                        }
                                                    }
                                                    : "$Matches"
                                            },
                                            { "sortBy", new BsonDocument("Date", -1) }
                                        }
                                    }
                                },
                                skip,
                                pageSize
                            }
                        }
                    }
                },
                {
                    "TotalMatches",
                    matchesFilter != null
                        ? new BsonDocument
                        {
                            {
                                "$size",
                                new BsonDocument
                                {
                                    {
                                        "$filter", new BsonDocument
                                        {
                                            { "input", "$Matches" },
                                            { "as", "match" },
                                            { "cond", matchesFilter }
                                        }
                                    }
                                }
                            }
                        }
                        : new BsonDocument("$size", "$Matches")
                }
            })
            .SingleOrDefaultAsync();

        if (player == null)
            return TypedResults.NotFound();
        
        var result = player.ToMatchHistoryResponse(characterStore);
        
        return TypedResults.Ok(result);
    }

    private BsonDocument? BuildMatchesFilter(int? characterId, int? opponentCharacterId, string? opponentTekkenId)
    {
        var conditions = new List<BsonDocument>();

        if (characterId.HasValue)
        {
            conditions.Add(new BsonDocument("$eq", new BsonArray
            {
                "$$match.Challenger.CharacterId",
                characterId.Value
            }));
        }

        if (opponentCharacterId.HasValue)
        {
            conditions.Add(new BsonDocument("$eq", new BsonArray
            {
                "$$match.Opponent.CharacterId",
                opponentCharacterId.Value
            }));
        }

        if (!string.IsNullOrEmpty(opponentTekkenId))
        {
            conditions.Add(new BsonDocument("$eq", new BsonArray
            {
                "$$match.Opponent.TekkenId",
                opponentTekkenId
            }));
        }

        if (conditions.Count == 0)
        {
            return null;
        }

        return conditions.Count == 1 
            ? conditions[0] 
            : new BsonDocument("$and", new BsonArray(conditions));
    }
}

public class PlayerMatchesProjection
{
    public List<Match> Matches { get; set; } = [];
    public int TotalMatches { get; set; }
}


