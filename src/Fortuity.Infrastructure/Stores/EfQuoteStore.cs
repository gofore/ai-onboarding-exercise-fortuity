using Fortuity.Application.Abstractions;
using Fortuity.Domain.Quotes;
using Fortuity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fortuity.Infrastructure.Stores;

internal sealed class EfQuoteStore(FortuityDbContext context) : IQuoteStore
{
    public async Task AddAsync(Quote quote, CancellationToken cancellationToken = default)
    {
        context.Quotes.Add(quote);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyList<Quote>> ListForCustomerAsync(int customerId, CancellationToken cancellationToken = default) =>
        await context.Quotes
            .Where(quote => quote.CustomerId == customerId)
            .OrderByDescending(quote => quote.Id)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
}
