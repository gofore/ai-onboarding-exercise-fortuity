using Fortuity.Domain.Policies;

namespace Fortuity.Application.Abstractions;

public interface IPolicyStore
{
    Task<Policy?> FindByNumberAsync(PolicyNumber number, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Policy>> ListForCustomerAsync(int customerId, CancellationToken cancellationToken = default);
}
