using Fortuity.Domain.Partners;

namespace Fortuity.Api.Contracts;

public sealed record SyncReceiptResponse(Guid TicketId, int ShopCount);

public sealed record RepairShopStatusResponse(
    string PartnerReference,
    string Name,
    string City,
    string Region,
    bool IsAcceptingWork,
    string SyncState,
    DateTimeOffset? LastSyncedAt,
    string? LastSyncError)
{
    internal static RepairShopStatusResponse From(RepairShop shop) => new(
        shop.PartnerReference,
        shop.Name,
        shop.City,
        shop.Region.Value,
        shop.IsAcceptingWork,
        shop.SyncState.ToString(),
        shop.LastSyncedAt,
        shop.LastSyncError);
}
