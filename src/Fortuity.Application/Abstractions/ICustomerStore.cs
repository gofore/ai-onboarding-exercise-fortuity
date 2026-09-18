using Fortuity.Domain.Customers;

namespace Fortuity.Application.Abstractions;

public interface ICustomerStore
{
    Task<Customer?> FindByNumberAsync(string customerNumber, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Customer>> ListAsync(CancellationToken cancellationToken = default);
}
