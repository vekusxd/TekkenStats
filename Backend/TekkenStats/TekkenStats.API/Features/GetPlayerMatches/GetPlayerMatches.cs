using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
        MongoDatabase db)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionary());

        var collection = db.Db.GetCollection<Player>(Player.CollectionName);

        var pageNumber = request.PageNumber ?? 1;
        var pageSize = request.PageSize ?? 10;
        var skip = (pageNumber - 1) * pageSize;

        var result = await collection.Aggregate()
            .Match(p => p.TekkenId == request.TekkenId)
            .Project<GetPlayerMatchesResponse>(new BsonDocument
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

        if (result == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(result);
    }
}

public class GetPlayerMatchesRequest
{
    [FromRoute] public required string TekkenId { get; set; }

    [FromQuery] public int? PageSize { get; set; } = 10;
    [FromQuery] public int? PageNumber { get; set; } = 1;
}

public class GetPlayerMatchesResponse
{
    public List<Match> Matches { get; set; } = [];
    public int TotalMatches { get; set; }
}