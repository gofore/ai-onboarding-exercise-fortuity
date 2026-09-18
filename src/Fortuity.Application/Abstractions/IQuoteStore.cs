using Fortuity.Domain.Quotes;

namespace Fortuity.Application.Abstractions;

public interface IQuoteStore
{
    Task AddAsync(Quote quote, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Quote>> ListForCustomerAsync(int customerId, CancellationToken cancellationToken = default);
}
