using Fortuity.Application.Underwriting;
using Fortuity.Domain.Common;
using Fortuity.Domain.Policies;
using Fortuity.Domain.Underwriting;
using Fortuity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fortuity.Infrastructure.Stores;

internal sealed class EfTariffSource(FortuityDbContext context) : ITariffSource
{
    public async Task<decimal?> ResolveMultiplierAsync(
        RegionCode region,
        PolicyType policyType,
        RatingBasis basis,
        DateOnly asOf,
        CancellationToken cancellationToken = default)
    {
        var factor = await context.RegionalFactors
            .Where(candidate => candidate.Region == region
                && candidate.PolicyType == policyType
                && candidate.Basis == basis
                && candidate.EffectiveFrom <= asOf)
            .OrderByDescending(candidate => candidate.EffectiveFrom)
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        return factor?.Multiplier;
    }
}
