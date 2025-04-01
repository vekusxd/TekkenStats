using FluentValidation;

namespace TekkenStats.API.Features.HeadToHead;

public class GetHeadToHeadRequestValidator : AbstractValidator<GetHeadToHeadRequest>
{
    public GetHeadToHeadRequestValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
    }
}