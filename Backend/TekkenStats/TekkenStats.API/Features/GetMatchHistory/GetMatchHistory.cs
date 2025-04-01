using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using TekkenStats.API.Features.Shared;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.GetMatchHistory;

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

        var collection = db.Db.GetCollection<Player>(Player.CollectionName);

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

        var result = new GetMatchHistoryResponse
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
                    TekkenId = tekkenId,
                    Name = m.Challenger.Name,
                    CharacterId = m.Challenger.CharacterId,
                    CharacterName = characterStore.GetCharacter(m.Challenger.CharacterId).Name,
                    Rounds = m.Challenger.Rounds,
                    RatingBefore = m.Challenger.RatingBefore,
                    RatingChange = m.Challenger.RatingChange,
                    CharacterImgURL = characterStore.GetCharacter(m.Challenger.CharacterId).ImgURL
                },
                Opponent = new ChallengerInfoResponse
                {
                    TekkenId = m.Opponent.TekkenId,
                    Name = m.Opponent.Name,
                    CharacterId = m.Opponent.CharacterId,
                    CharacterName = characterStore.GetCharacter(m.Opponent.CharacterId).Name,
                    Rounds = m.Opponent.Rounds,
                    RatingBefore = m.Opponent.RatingBefore,
                    RatingChange = m.Opponent.RatingChange,
                    CharacterImgURL = characterStore.GetCharacter(m.Opponent.CharacterId).ImgURL,
                }
            }).ToList(),
        };
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

public class GetMatchHistoryRequest
{
    [FromQuery] public int? PageSize { get; set; } = 10;
    [FromQuery] public int? PageNumber { get; set; } = 1;
    [FromQuery] public int? CharacterId { get; init; }
    [FromQuery] public int? OpponentCharacterId { get; init; }
}

public class PlayerMatchesProjection
{
    public List<Match> Matches { get; set; } = [];
    public int TotalMatches { get; set; }
}

public class GetMatchHistoryResponse
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
    public required ChallengerInfoResponse Opponent { get; init; }
}

public class ChallengerInfoResponse
{
    public int CharacterId { get; init; }
    public required string TekkenId { get; init; }
    public required string Name { get; init; }
    public required string CharacterName { get; init; }
    public required string CharacterImgURL { get; init; }
    public int Rounds { get; init; }
    public int RatingBefore { get; init; }
    public int RatingChange { get; init; }
}