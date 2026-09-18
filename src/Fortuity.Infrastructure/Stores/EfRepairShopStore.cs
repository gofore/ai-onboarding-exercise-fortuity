using Fortuity.Application.Abstractions;
using Fortuity.Domain.Partners;
using Fortuity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fortuity.Infrastructure.Stores;

internal sealed class EfRepairShopStore(FortuityDbContext context) : IRepairShopStore
{
    public async Task<IReadOnlyList<RepairShop>> ListAsync(CancellationToken cancellationToken = default) =>
        await context.RepairShops.OrderBy(shop => shop.Name).ToListAsync(cancellationToken).ConfigureAwait(false);

    public async Task<IReadOnlyList<RepairShop>> ListByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default) =>
        await context.RepairShops.Where(shop => ids.Contains(shop.Id)).ToListAsync(cancellationToken).ConfigureAwait(false);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
