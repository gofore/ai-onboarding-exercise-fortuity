using Fortuity.Application.Abstractions;
using Fortuity.Domain.Policies;
using Fortuity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fortuity.Infrastructure.Stores;

internal sealed class EfPolicyStore(FortuityDbContext context) : IPolicyStore
{
    public Task<Policy?> FindByNumberAsync(PolicyNumber number, CancellationToken cancellationToken = default) =>
        context.Policies.SingleOrDefaultAsync(policy => policy.Number == number, cancellationToken);

    public async Task<IReadOnlyList<Policy>> ListForCustomerAsync(int customerId, CancellationToken cancellationToken = default) =>
        await context.Policies
            .Where(policy => policy.CustomerId == customerId)
            .OrderBy(policy => policy.StartsOn)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
