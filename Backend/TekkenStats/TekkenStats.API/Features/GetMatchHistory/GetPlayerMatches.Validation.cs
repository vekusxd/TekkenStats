using FluentValidation;

namespace TekkenStats.API.Features.GetMatchHistory;

public class GetPlayerMatchesValidator : AbstractValidator<GetPlayerMatchHistoryRequest>
{
    public GetPlayerMatchesValidator()
    {
        RuleFor(x => x.TekkenId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
    }
}