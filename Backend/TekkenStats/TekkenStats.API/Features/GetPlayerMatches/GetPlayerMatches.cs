using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.GetPlayerMatches;

public class GetPlayerMatches : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/matches/{tekkenId}", Handler);
    }

    private async Task<Results<Ok<GetPlayerMatchesResponse>, NotFound, ValidationProblem>> Handler(
        [AsParameters] GetPlayerMatchesRequest request,
        IValidator<GetPlayerMatchesRequest> validator,
        MongoDatabase db,
        IMemoryCache cache)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var collection = db.Db.GetCollection<Player>(Player.CollectionName);

        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;
        var skip = (pageNumber - 1) * pageSize;

        var player = await collection.Aggregate()
            .Match(p => p.TekkenId == request.TekkenId)
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
                                            { "input", "$Matches" },
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
                { "TotalMatches", new BsonDocument("$size", "$Matches") }
            })
            .SingleOrDefaultAsync();

        if (player == null)
            return TypedResults.NotFound();

        var result = new GetPlayerMatchesResponse
        {
            TotalMatches = player.TotalMatches,
            Matches = player.Matches.Select(m => new MatchResponse
            {
                BattleId = m.BattleId,
                Date = m.Date,
                Winner = m.Winner,
                GameVersion = m.GameVersion,
                Challenger = new ChallengerInfoResponse
                {
                    CharacterId = m.Challenger.CharacterId,
                    CharacterName = cache.Get<string>(m.Challenger.CharacterId) ??
                                    throw new NullReferenceException(
                                        $"Character with id: {m.Challenger.CharacterId} not found"),
                    Rounds = m.Challenger.Rounds,
                    RatingBefore = m.Challenger.RatingBefore,
                    RatingChange = m.Challenger.RatingChange
                },
                Opponent = new OpponentInfoResponse
                {
                    TekkenId = m.Opponent.TekkenId,
                    CharacterId = m.Opponent.CharacterId,
                    CharacterName = cache.Get<string>(m.Opponent.CharacterId) ??
                                    throw new NullReferenceException(
                                        $"Character with id: {m.Opponent.CharacterId} not found"),
                    Rounds = m.Opponent.Rounds,
                    RatingBefore = m.Opponent.RatingBefore,
                    RatingChange = m.Opponent.RatingChange
                }
            }).ToList(),
        };
        return TypedResults.Ok(result);
    }
}

public class GetPlayerMatchesRequest
{
    [FromRoute] public required string TekkenId { get; set; }

    [FromQuery] public int? PageSize { get; set; } = 10;
    [FromQuery] public int? PageNumber { get; set; } = 1;
}

public class PlayerMatchesProjection
{
    public List<Match> Matches { get; set; } = [];
    public int TotalMatches { get; set; }
}

public class GetPlayerMatchesResponse
{
    public List<MatchResponse> Matches { get; set; } = [];
    public int TotalMatches { get; set; }
}

public class MatchResponse
{
    public required string BattleId { get; init; }
    public DateTime Date { get; init; }
    public long GameVersion { get; init; }
    public bool Winner { get; init; }
    public required ChallengerInfoResponse Challenger { get; init; }
    public required OpponentInfoResponse Opponent { get; init; }
}

public class ChallengerInfoResponse
{
    public int CharacterId { get; init; }
    public required string CharacterName { get; init; }
    public int Rounds { get; init; }
    public int RatingBefore { get; init; }
    public int RatingChange { get; init; }
}

public class OpponentInfoResponse
{
    public int CharacterId { get; init; }
    public required string TekkenId { get; init; }
    public required string CharacterName { get; init; }
    public int Rounds { get; init; }
    public int RatingBefore { get; init; }
    public int RatingChange { get; init; }
}