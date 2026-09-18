using Fortuity.Domain.Partners;

namespace Fortuity.Application.Abstractions;

public interface IRepairShopStore
{
    Task<IReadOnlyList<RepairShop>> ListAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RepairShop>> ListByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
