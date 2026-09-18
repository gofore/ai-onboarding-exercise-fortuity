using Fortuity.Application.Common;
using Fortuity.Domain.Claims;

namespace Fortuity.Application.ClaimsIntake;

public interface IFnolProcessor
{
    Task<Result<Claim>> RegisterAsync(FnolReport report, CancellationToken cancellationToken = default);
}
