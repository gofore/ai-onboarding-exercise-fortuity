using Fortuity.Application.Abstractions;
using Fortuity.Application.Common;
using Fortuity.Domain.Claims;

namespace Fortuity.Application.ClaimsIntake;

public sealed class AuditedFnolProcessor(IFnolProcessor inner, IAuditTrail auditTrail) : IFnolProcessor
{
    public async Task<Result<Claim>> RegisterAsync(FnolReport report, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(report);

        var result = await inner.RegisterAsync(report, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            auditTrail.Record(
                "ClaimRegistered",
                result.Value.Number.Value,
                $"{result.Value.Type} on policy {report.PolicyNumber}, reserve {result.Value.ReserveAmount}");
        }
        else
        {
            auditTrail.Record("ClaimRejected", report.PolicyNumber, result.Error.Code);
        }

        return result;
    }
}
