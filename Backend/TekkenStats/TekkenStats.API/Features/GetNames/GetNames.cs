using Microsoft.AspNetCore.Mvc;

namespace TekkenStats.API.Features.GetNames;

//TODO
public class GetNames : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
    }
}

public class GetNamesRequest
{
    [FromQuery] public int? Amount { get; init; } = 10;
    [FromQuery] public required string Name { get; init; }
}

public class GetNamesResponse
{
    public required string Name { get; init; }
    public required string PlayerId { get; init; }
}