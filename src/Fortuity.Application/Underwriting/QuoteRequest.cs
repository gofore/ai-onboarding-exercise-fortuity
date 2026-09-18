using FluentValidation;
using Fortuity.Domain.Policies;

namespace Fortuity.Application.Underwriting;

public sealed record QuoteRequest(
    string CustomerNumber,
    PolicyType PolicyType,
    CoverageLevel Coverage,
    int AgeYears,
    int NoClaimsYears);

public sealed class QuoteRequestValidator : AbstractValidator<QuoteRequest>
{
    public QuoteRequestValidator()
    {
        RuleFor(request => request.CustomerNumber).NotEmpty().MaximumLength(20);
        RuleFor(request => request.PolicyType).IsInEnum();
        RuleFor(request => request.Coverage).IsInEnum();
        RuleFor(request => request.AgeYears).InclusiveBetween(0, 99);
        RuleFor(request => request.NoClaimsYears).InclusiveBetween(0, 40);
    }
}
