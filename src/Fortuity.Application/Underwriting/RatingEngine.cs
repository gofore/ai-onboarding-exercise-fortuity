using Fortuity.Application.Common;
using Fortuity.Domain.Common;
using Fortuity.Domain.Policies;
using Fortuity.Domain.Quotes;

namespace Fortuity.Application.Underwriting;

public sealed class RatingEngine(ITariffSource tariffSource)
{
    private static readonly Money MotorBasePremium = Money.FromEuros(420m);
    private static readonly Money HomeBasePremium = Money.FromEuros(260m);

    public async Task<Result<PremiumBreakdown>> RateAsync(RatingRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var regionalMultiplier = await tariffSource
            .ResolveMultiplierAsync(request.Region, request.PolicyType, request.Basis, request.AsOf, cancellationToken)
            .ConfigureAwait(false);

        if (regionalMultiplier is null)
        {
            return Result.Failure<PremiumBreakdown>(Error.NotFound(
                "tariff.not_found",
                $"No {request.Basis} tariff exists for region {request.Region} and {request.PolicyType} on {request.AsOf:yyyy-MM-dd}."));
        }

        var basePremium = BasePremiumFor(request.PolicyType);
        var riskMultiplier = RiskMultiplierFor(request.PolicyType, request.AgeYears);
        var coverageMultiplier = CoverageMultiplierFor(request.Coverage);
        var noClaimsMultiplier = NoClaimsMultiplierFor(request.NoClaimsYears);

        var finalPremium = basePremium
            .MultiplyBy(riskMultiplier)
            .MultiplyBy(coverageMultiplier)
            .MultiplyBy(noClaimsMultiplier)
            .MultiplyBy(regionalMultiplier.Value);

        return Result.Success(new PremiumBreakdown(
            basePremium,
            riskMultiplier,
            coverageMultiplier,
            noClaimsMultiplier,
            regionalMultiplier.Value,
            finalPremium));
    }

    private static Money BasePremiumFor(PolicyType policyType) => policyType switch
    {
        PolicyType.Motor => MotorBasePremium,
        PolicyType.Home => HomeBasePremium,
        _ => throw new ArgumentOutOfRangeException(nameof(policyType), policyType, "Unknown policy type."),
    };

    private static decimal RiskMultiplierFor(PolicyType policyType, int ageYears) => policyType switch
    {
        PolicyType.Motor => ageYears switch
        {
            <= 3 => 1.15m,
            <= 10 => 1.00m,
            <= 20 => 0.92m,
            _ => 1.05m,
        },
        PolicyType.Home => ageYears switch
        {
            <= 10 => 0.95m,
            <= 30 => 1.00m,
            <= 60 => 1.10m,
            _ => 1.25m,
        },
        _ => 1.00m,
    };

    private static decimal CoverageMultiplierFor(CoverageLevel coverage) => coverage switch
    {
        CoverageLevel.Basic => 0.85m,
        CoverageLevel.Standard => 1.00m,
        CoverageLevel.Plus => 1.25m,
        _ => 1.00m,
    };

    private static decimal NoClaimsMultiplierFor(int noClaimsYears) =>
        1.00m - (0.03m * Math.Min(Math.Max(noClaimsYears, 0), 10));
}
