namespace Fortuity.Application.PartnerSync;

public sealed record SyncTicket(
    Guid TicketId,
    IReadOnlyList<int> RepairShopIds,
    DateTimeOffset RequestedAt);
