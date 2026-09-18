using Fortuity.Domain.Common;
using Fortuity.Domain.Policies;
using Fortuity.Domain.Underwriting;

namespace Fortuity.Domain.Quotes;

public sealed class Quote
{
    public int Id { get; private set; }

    public required int CustomerId { get; init; }

    public required PolicyType PolicyType { get; init; }

    public required CoverageLevel Coverage { get; init; }

    public required RegionCode Region { get; init; }

    public required int VehicleOrPropertyAgeYears { get; init; }

    public required int NoClaimsYears { get; init; }

    public required RatingBasis BasisUsed { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public PremiumBreakdown Breakdown { get; set; } = PremiumBreakdown.Empty;

    public Money AnnualPremium => Breakdown.FinalPremium;
}
