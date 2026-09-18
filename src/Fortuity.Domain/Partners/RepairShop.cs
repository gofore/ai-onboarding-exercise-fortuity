using Fortuity.Domain.Common;

namespace Fortuity.Domain.Partners;

public sealed class RepairShop
{
    public int Id { get; private set; }

    public required string PartnerReference { get; init; }

    public required string Name { get; init; }

    public required string City { get; init; }

    public required RegionCode Region { get; init; }

    public bool IsAcceptingWork { get; set; } = true;

    public ShopSyncState SyncState { get; set; } = ShopSyncState.NeverSynced;

    public DateTimeOffset? LastSyncedAt { get; set; }

    public string? LastSyncError { get; set; }
}
