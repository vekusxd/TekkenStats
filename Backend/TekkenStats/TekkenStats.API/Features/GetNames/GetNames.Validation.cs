using FluentValidation;

namespace TekkenStats.API.Features.GetNames;

public class GetNamesValidator : AbstractValidator<GetNamesRequest>
{
    public GetNamesValidator()
    {
        RuleFor(r => r.Amount).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
        RuleFor(r => r.StartsWith).MaximumLength(30).NotEmpty().NotNull();
    }
}