using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using TekkenStats.API.Features.Shared;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.HeadToHead;

public class GetHeadToHead : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/head-to-head/{tekkenId}/{opponentTekkenId}", Handler);
    }

    private async Task<Results<Ok<GetHeadToHeadResponse>, NotFound, ValidationProblem>> Handler(
        string tekkenId,
        string opponentTekkenId,
        [AsParameters] GetHeadToHeadRequest request,
        IValidator<GetHeadToHeadRequest> validator,
        MongoDatabase db,
        CharacterStore characterStore)
    {
        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;
        var skip = (pageNumber - 1) * pageSize;

        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var collection = db.Db.GetCollection<Player>(Player.CollectionName);

        var matchesFilter = BuildMatchesFilter(request.CharacterId, request.OpponentCharacterId, opponentTekkenId);

        var pipeline = collection.Aggregate()
            .Match(p => p.TekkenId == tekkenId);

        var statsPipeline = new List<BsonDocument>
        {
            new("$project", new BsonDocument
            {
                {
                    "Matches", matchesFilter != null
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
                }
            }),
            new("$addFields", new BsonDocument
            {
                { "TotalMatches", new BsonDocument("$size", "$Matches") },
                {
                    "WinCount", new BsonDocument("$size", new BsonDocument("$filter", new BsonDocument
                    {
                        { "input", "$Matches" },
                        { "as", "match" },
                        { "cond", new BsonDocument("$eq", new BsonArray { "$$match.Winner", true }) }
                    }))
                },
                {
                    "LossCount", new BsonDocument("$size", new BsonDocument("$filter", new BsonDocument
                    {
                        { "input", "$Matches" },
                        { "as", "match" },
                        { "cond", new BsonDocument("$eq", new BsonArray { "$$match.Winner", false }) }
                    }))
                }
            }),
            new("$project", new BsonDocument
            {
                { "_id", 0 },
                {
                    "Matches", new BsonDocument("$slice", new BsonArray
                    {
                        new BsonDocument("$sortArray", new BsonDocument
                        {
                            { "input", "$Matches" },
                            { "sortBy", new BsonDocument("Date", -1) }
                        }),
                        skip,
                        pageSize
                    })
                },
                { "TotalMatches", 1 },
                { "WinCount", 1 },
                { "LossCount", 1 }
            })
        };

        var result = await pipeline.AppendStage<HeadToHeadMatchesProjection>(statsPipeline.First())
            .AppendStage<HeadToHeadMatchesProjection>(statsPipeline[1])
            .AppendStage<HeadToHeadMatchesProjection>(statsPipeline[2])
            .SingleOrDefaultAsync();

        if (result == null)
            return TypedResults.NotFound();

        var response = new GetHeadToHeadResponse
        {
            Matches = result.Matches.Select(m => new MatchResponse
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
            TotalMatches = result.TotalMatches,
            WinCount = result.WinCount,
            LossCount = result.LossCount
        };

        return TypedResults.Ok(response);
    }

    private BsonDocument? BuildMatchesFilter(int? characterId, int? opponentCharacterId, string opponentTekkenId)
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

public class GetHeadToHeadRequest
{
    [FromQuery] public int? PageSize { get; set; } = 10;
    [FromQuery] public int? PageNumber { get; set; } = 1;
    [FromQuery] public int? CharacterId { get; init; }
    [FromQuery] public int? OpponentCharacterId { get; init; }
}

public class GetHeadToHeadResponse
{
    public List<MatchResponse> Matches { get; set; } = [];
    public int TotalMatches { get; set; }
    public int WinCount { get; init; }
    public int LossCount { get; init; }
    public double ChallengerWinRate => TotalMatches != 0 ? Math.Round((double)WinCount / TotalMatches * 100, 2) : 0;
    public double OpponentWinRate => TotalMatches != 0 ? 100 - ChallengerWinRate : 0;
}

public class HeadToHeadMatchesProjection
{
    public List<Match> Matches { get; set; } = [];
    public int TotalMatches { get; set; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
}