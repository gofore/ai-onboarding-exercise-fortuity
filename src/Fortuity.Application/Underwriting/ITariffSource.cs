using Fortuity.Domain.Common;
using Fortuity.Domain.Policies;
using Fortuity.Domain.Underwriting;

namespace Fortuity.Application.Underwriting;

public interface ITariffSource
{
    Task<decimal?> ResolveMultiplierAsync(
        RegionCode region,
        PolicyType policyType,
        RatingBasis basis,
        DateOnly asOf,
        CancellationToken cancellationToken = default);
}
