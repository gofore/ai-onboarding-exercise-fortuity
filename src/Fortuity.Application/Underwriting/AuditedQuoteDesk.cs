using Fortuity.Application.Abstractions;
using Fortuity.Application.Common;
using Fortuity.Domain.Quotes;

namespace Fortuity.Application.Underwriting;

public sealed class AuditedQuoteDesk(IQuoteDesk inner, IAuditTrail auditTrail) : IQuoteDesk
{
    public async Task<Result<Quote>> DraftAsync(QuoteRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await inner.DraftAsync(request, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            auditTrail.Record(
                "QuoteDrafted",
                request.CustomerNumber,
                $"{result.Value.PolicyType}/{result.Value.Coverage} on {result.Value.BasisUsed} basis, premium {result.Value.AnnualPremium}");
        }
        else
        {
            auditTrail.Record("QuoteRejected", request.CustomerNumber, result.Error.Code);
        }

        return result;
    }
}
