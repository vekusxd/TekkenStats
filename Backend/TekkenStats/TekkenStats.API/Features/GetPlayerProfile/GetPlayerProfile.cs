using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using MongoDB.Driver;
using TekkenStats.API.Features.Shared;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.GetPlayerProfile;

public class GetPlayerProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/profile/{TekkenId}", Handler);
    }

    private async Task<Results<Ok<GetPlayerProfileResponse>, NotFound, ValidationProblem>> Handler(
        [AsParameters] GetPlayerProfileRequest request,
        IValidator<GetPlayerProfileRequest> validator,
        MongoDatabase database,
        CharacterStore characterStore
    )
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var collection = database.Db.GetCollection<Player>(Player.CollectionName);

        var player = await collection.Aggregate()
            .Match(p => p.TekkenId == request.TekkenId)
            .Project<GetPlayerProfileProjection>(new BsonDocument
            {
                { "_id", 0 },
                { "Power", 1 },
                { "Characters", 1 },
                { "CurrentName", 1 },
                {
                    "Names",
                    new BsonDocument("$sortArray",
                        new BsonDocument
                        {
                            { "input", "$Names" },
                            { "sortBy", new BsonDocument("Date", -1) }
                        })
                },
                { "MatchesCount", new BsonDocument("$size", "$Matches") },
                {
                    "WinCount",
                    new BsonDocument("$size",
                        new BsonDocument("$filter",
                            new BsonDocument
                            {
                                { "input", "$Matches" },
                                { "as", "match" },
                                { "cond", new BsonDocument("$eq", new BsonArray { "$$match.Winner", true }) }
                            }))
                }
            })
            .AppendStage<GetPlayerProfileProjection>(new BsonDocument("$addFields",
                new BsonDocument
                {
                    { "LossCount", new BsonDocument("$subtract", new BsonArray { "$MatchesCount", "$WinCount" }) },
                    {
                        "Characters",
                        new BsonDocument("$sortArray",
                            new BsonDocument
                            {
                                { "input", "$Characters" },
                                { "sortBy", new BsonDocument("MatchesCount", -1) }
                            })
                    }
                }))
            .SingleOrDefaultAsync();

        if (player == null)
            return TypedResults.NotFound();

        var result = new GetPlayerProfileResponse
        {
            TekkenId = request.TekkenId,
            CurrentName = player.CurrentName,
            LossCount = player.LossCount,
            WinCount = player.WinCount,
            MatchesCount = player.MatchesCount,
            Names = player.Names,
            Power = player.Power,
            Characters = player.Characters.Select(c => new CharacterResponse
            {
                CharacterId = c.CharacterId,
                CharacterName = characterStore.GetCharacter(c.CharacterId).Name ??
                                throw new NullReferenceException($"Character with id: {c.CharacterId} not found"),
                MatchesCount = c.MatchesCount,
                WinCount = c.WinCount,
                LossCount = c.LossCount,
                Rating = c.Rating,
                LastPlayed = c.LastPlayed,
                ImgURL = characterStore.GetCharacter(c.CharacterId).ImgURL ??
                         throw new Exception($"Character with id: {c.CharacterId} not found")
            }).ToList(),
        };

        return TypedResults.Ok(result);
    }
}

public class GetPlayerProfileRequest
{
    [FromRoute] public required string TekkenId { get; set; }
}

public class GetPlayerProfileProjection
{
    public required string CurrentName { get; init; }
    public long Power { get; set; }
    public int MatchesCount { get; set; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public List<CharacterInfo> Characters { get; init; } = [];
    public List<Name> Names { get; set; } = [];
}

public class GetPlayerProfileResponse
{
    public required string TekkenId { get; init; }
    public required string CurrentName { get; init; }
    public long Power { get; init; }
    public int MatchesCount { get; init; }
    public int WinCount { get; init; }
    public int LossCount { get; init; }
    public List<CharacterResponse> Characters { get; init; } = [];
    public List<Name> Names { get; init; } = [];
}

public class CharacterResponse
{
    public required int CharacterId { get; init; }
    public required string CharacterName { get; init; }
    public required string ImgURL { get; init; }
    public int MatchesCount { get; set; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public int Rating { get; set; }
    public DateTime LastPlayed { get; set; }
    public double WinRate => Math.Round((double)WinCount / MatchesCount, 2) * 100;
}