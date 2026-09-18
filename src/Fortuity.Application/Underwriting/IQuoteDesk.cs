using Fortuity.Application.Common;
using Fortuity.Domain.Quotes;

namespace Fortuity.Application.Underwriting;

public interface IQuoteDesk
{
    Task<Result<Quote>> DraftAsync(QuoteRequest request, CancellationToken cancellationToken = default);
}
