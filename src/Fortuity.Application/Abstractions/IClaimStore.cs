using Fortuity.Domain.Claims;

namespace Fortuity.Application.Abstractions;

public interface IClaimStore
{
    Task AddAsync(Claim claim, CancellationToken cancellationToken = default);

    Task<int> NextSequenceAsync(int year, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Claim>> ListOpenAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Claim>> ListOpenForShopAsync(int repairShopId, CancellationToken cancellationToken = default);
}
