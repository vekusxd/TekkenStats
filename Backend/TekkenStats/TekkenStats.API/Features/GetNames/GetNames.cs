using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.GetNames;

public class GetNames : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/names", Handler);
    }

    private async Task<Results<Ok<List<GetNamesResponse>>, ValidationProblem>> Handler(
        IValidator<GetNamesRequest> validator,
        [AsParameters] GetNamesRequest request,
        MongoDatabase database)
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());

        var collection = database.Db.GetCollection<Player>(Player.CollectionName);

        var names = await collection
            .Aggregate()
            .Project<Player, ProjectedPlayer>(p => new ProjectedPlayer
            {
                Id = p.TekkenId,
                Names = p.Names
            }).Unwind(p => p.Names,
                new AggregateUnwindOptions<UnwoundPlayer> { PreserveNullAndEmptyArrays = true })
            .Project<UnwoundPlayer, GetNamesResponse>(p => new GetNamesResponse
            {
                TekkenId = p.Id,
                Name = p.Name.PlayerName,
            })
            .Match(p => Regex.IsMatch(p.Name, $"^{request.StartsWith}", RegexOptions.IgnoreCase))
            .Limit(15)
            .ToListAsync();

        return TypedResults.Ok(names);
    }

    private class ProjectedPlayer
    {
        [BsonElement("_id")] public required string Id { get; set; }
        public required List<Name> Names { get; set; }
    }

    private class UnwoundPlayer
    {
        [BsonElement("_id")] public required string Id { get; set; }
        [BsonElement("Names")] public required Name Name { get; set; }
    }
}

public class GetNamesRequest
{
    [FromQuery] public int? Amount { get; init; } = 10;
    [FromQuery] public required string StartsWith { get; init; }
}

public class GetNamesResponse
{
    [BsonElement("_id")] public required string TekkenId { get; init; }
    public required string Name { get; init; }
}