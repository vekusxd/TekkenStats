using FluentValidation;

namespace TekkenStats.API.Features.GetRivals;

public class GetRivalsRequestValidator : AbstractValidator<GetRivalsRequest>
{
    public GetRivalsRequestValidator()
    {
        RuleFor(r => r.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
        RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
    }
}