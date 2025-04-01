using FluentValidation;

namespace TekkenStats.API.Features.GetMatchHistory;

public class GetMatchHistoryRequestValidator : AbstractValidator<GetMatchHistoryRequest>
{
    public GetMatchHistoryRequestValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);
    }
}