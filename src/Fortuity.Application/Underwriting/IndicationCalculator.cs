using Fortuity.Application.Common;
using Fortuity.Domain.Common;
using Fortuity.Domain.Policies;
using Fortuity.Domain.Quotes;
using Fortuity.Domain.Underwriting;

namespace Fortuity.Application.Underwriting;

public sealed record IndicationRequest(
    PolicyType PolicyType,
    CoverageLevel Coverage,
    RegionCode Region,
    RatingBasis Basis,
    int AgeYears,
    int NoClaimsYears);

public sealed class IndicationCalculator(RatingEngine ratingEngine, IClock clock)
{
    public Task<Result<PremiumBreakdown>> EstimateAsync(IndicationRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        return ratingEngine.RateAsync(
            new RatingRequest(
                request.PolicyType,
                request.Coverage,
                request.Region,
                request.Basis,
                request.AgeYears,
                request.NoClaimsYears,
                clock.Today),
            cancellationToken);
    }
}
