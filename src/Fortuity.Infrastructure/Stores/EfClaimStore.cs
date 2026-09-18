using Fortuity.Application.Abstractions;
using Fortuity.Domain.Claims;
using Fortuity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fortuity.Infrastructure.Stores;

internal sealed class EfClaimStore(FortuityDbContext context) : IClaimStore
{
    public async Task AddAsync(Claim claim, CancellationToken cancellationToken = default)
    {
        context.Claims.Add(claim);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<int> NextSequenceAsync(int year, CancellationToken cancellationToken = default)
    {
        var prefix = $"CLM-{year:D4}-";
        var numbers = await context.Claims
            .Select(claim => claim.Number)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return numbers.Count(number => number.Value.StartsWith(prefix, StringComparison.Ordinal)) + 1;
    }

    public async Task<IReadOnlyList<Claim>> ListOpenAsync(CancellationToken cancellationToken = default) =>
        await context.Claims
            .Where(claim => claim.Status != ClaimStatus.Settled && claim.Status != ClaimStatus.Rejected)
            .OrderBy(claim => claim.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task<IReadOnlyList<Claim>> ListOpenForShopAsync(int repairShopId, CancellationToken cancellationToken = default) =>
        await context.Claims
            .Where(claim => claim.AssignedRepairShopId == repairShopId
                && claim.Status != ClaimStatus.Settled
                && claim.Status != ClaimStatus.Rejected)
            .OrderBy(claim => claim.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
