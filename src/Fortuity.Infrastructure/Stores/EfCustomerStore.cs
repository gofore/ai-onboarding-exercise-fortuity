using Fortuity.Application.Abstractions;
using Fortuity.Domain.Customers;
using Fortuity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fortuity.Infrastructure.Stores;

internal sealed class EfCustomerStore(FortuityDbContext context) : ICustomerStore
{
    public Task<Customer?> FindByNumberAsync(string customerNumber, CancellationToken cancellationToken = default) =>
        context.Customers.SingleOrDefaultAsync(customer => customer.CustomerNumber == customerNumber, cancellationToken);

    public async Task<IReadOnlyList<Customer>> ListAsync(CancellationToken cancellationToken = default) =>
        await context.Customers.OrderBy(customer => customer.CustomerNumber).ToListAsync(cancellationToken).ConfigureAwait(false);
}
