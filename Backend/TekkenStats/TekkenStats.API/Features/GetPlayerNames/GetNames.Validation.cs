using FluentValidation;

namespace TekkenStats.API.Features.GetPlayerNames;

public class GetNamesValidator : AbstractValidator<GetNamesRequest>
{
    public GetNamesValidator()
    {
        RuleFor(r => r.Amount).GreaterThanOrEqualTo(1).LessThanOrEqualTo(15);
        RuleFor(r => r.StartsWith).MaximumLength(15).NotEmpty().NotNull();
    }
}