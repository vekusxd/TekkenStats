using FluentValidation;

namespace TekkenStats.API.Features.GetPlayerProfile;

public class GetPlayerProfileValidator : AbstractValidator<GetPlayerProfileRequest>
{
    public GetPlayerProfileValidator()
    {
        RuleFor(r => r.TekkenId)
            .NotEmpty()
            .NotNull()
            .MinimumLength(8)
            .MaximumLength(20);
    }
}