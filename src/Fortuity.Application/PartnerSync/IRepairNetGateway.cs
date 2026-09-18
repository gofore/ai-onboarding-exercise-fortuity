using Fortuity.Domain.Claims;
using Fortuity.Domain.Partners;

namespace Fortuity.Application.PartnerSync;

public sealed record RepairNetReceipt(string ConfirmationCode, int AcceptedCount);

public interface IRepairNetGateway
{
    Task<RepairNetReceipt> PushManifestAsync(
        RepairShop shop,
        IReadOnlyList<Claim> claims,
        CancellationToken cancellationToken = default);
}
