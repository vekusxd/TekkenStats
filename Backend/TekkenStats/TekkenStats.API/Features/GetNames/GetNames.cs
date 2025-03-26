using System.Globalization;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.GetNames;

public class GetNames : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/names", Handler);
    }

    private static async Task<Results<ValidationProblem, Ok<IEnumerable<GetNamesResponse>>>> Handler(
        [AsParameters] GetNamesRequest request,
        IValidator<GetNamesRequest> validator,
        AppDbContext dbContext)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var names = await dbContext.PlayerNames.AsNoTracking()
            .Where(p => p.NormalizedName.StartsWith(request.Name.ToUpperInvariant()))
            .OrderBy(p => p.Date)
            .Take(request.Amount ?? 10)
            .ToListAsync();

        return TypedResults.Ok(names.Select(n => n.ToResponse()));
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