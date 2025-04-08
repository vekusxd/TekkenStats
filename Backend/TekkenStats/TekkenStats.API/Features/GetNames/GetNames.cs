using System.Text.RegularExpressions;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Nodes;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;
using TekkenStats.DataAccess.Models;
using Name = TekkenStats.Core.Entities.Name;

namespace TekkenStats.API.Features.GetNames;

public class GetNamesRequest
{
    [FromQuery] public int? Amount { get; init; } = 10;
    [FromQuery] public required string StartsWith { get; init; }
}

public class GetNamesResponse
{
    public required string TekkenId { get; init; }
    public required string Name { get; init; }
}

public class GetNames : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/names", Handler);
    }

    private async Task<Results<Ok<List<GetNamesResponse>>, ValidationProblem>> Handler(
        IValidator<GetNamesRequest> validator,
        [AsParameters] GetNamesRequest request,
        MongoDatabase db,
        ElasticSearch elasticSearch
    )
    {
        var validation = await validator.ValidateAsync(request);
        if (!validation.IsValid)
            return TypedResults.ValidationProblem(validation.ToDictionary());


        var response = await elasticSearch.Client.SearchAsync<IndexedPlayer>(s => s
            .Index(IndexedPlayer.IndexName)
            .Size(request.Amount)
            .Query(q => q.Wildcard(w => w
                .Field(f => f.Name)
                .Value($"*{request.StartsWith}*")
                .CaseInsensitive())));

        var result = response.Documents.Select(p => new GetNamesResponse
        {
            TekkenId = p.TekkenId,
            Name = p.Name,
        }).ToList();

        return TypedResults.Ok(result);
    }
}