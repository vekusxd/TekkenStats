using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
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
        MongoDatabase database
    )
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var collection = database.Db.GetCollection<Player>(Player.CollectionName);

        var player = await collection.Aggregate()
            .Match(p => p.TekkenId == request.TekkenId)
            .Project<GetPlayerProfileResponse>(new BsonDocument
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
            .AppendStage<GetPlayerProfileResponse>(new BsonDocument("$addFields",
                new BsonDocument("LossCount",
                    new BsonDocument("$subtract", new BsonArray { "$MatchesCount", "$WinCount" }))))
            .SingleOrDefaultAsync();

        if (player == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(player);
    }
}

public class GetPlayerProfileRequest
{
    [FromRoute]
    public required string TekkenId { get; set; }
}

public class GetPlayerProfileResponse
{
    public required string CurrentName { get; init; }
    public long Power { get; set; }
    public int MatchesCount { get; set; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public List<CharacterInfo> Characters { get; init; } = [];
    public List<Name> Names { get; set; } = [];
}