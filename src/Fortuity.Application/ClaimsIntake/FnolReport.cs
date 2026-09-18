using FluentValidation;
using Fortuity.Domain.Claims;

namespace Fortuity.Application.ClaimsIntake;

public sealed record FnolReport(
    string PolicyNumber,
    ClaimType Type,
    DateOnly IncidentDate);

public sealed class FnolReportValidator : AbstractValidator<FnolReport>
{
    public FnolReportValidator()
    {
        RuleFor(report => report.PolicyNumber).NotEmpty().MaximumLength(20);
        RuleFor(report => report.Type).IsInEnum();
        RuleFor(report => report.IncidentDate)
            .GreaterThan(new DateOnly(2000, 1, 1))
            .WithMessage("Incident date is implausibly far in the past.");
    }
}
