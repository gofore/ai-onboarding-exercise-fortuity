using Fortuity.Domain.Common;

namespace Fortuity.Domain.Quotes;

public sealed record PremiumBreakdown(
    Money BasePremium,
    decimal RiskMultiplier,
    decimal CoverageMultiplier,
    decimal NoClaimsMultiplier,
    decimal RegionalMultiplier,
    Money FinalPremium)
{
    public static PremiumBreakdown Empty { get; } = new(
        Money.Zero,
        RiskMultiplier: 1m,
        CoverageMultiplier: 1m,
        NoClaimsMultiplier: 1m,
        RegionalMultiplier: 1m,
        Money.Zero);
}
