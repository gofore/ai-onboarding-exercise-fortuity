using Fortuity.Application.Abstractions;
using Fortuity.Application.Common;
using Fortuity.Domain.Partners;

namespace Fortuity.Application.PartnerSync;

public sealed record SyncReceipt(Guid TicketId, int ShopCount);

public sealed class SyncCoordinator(
    IRepairShopStore repairShopStore,
    ISyncBacklog backlog,
    IAuditTrail auditTrail,
    IClock clock)
{
    public async Task<Result<SyncReceipt>> RequestSyncAsync(CancellationToken cancellationToken = default)
    {
        var shops = await repairShopStore.ListAsync(cancellationToken).ConfigureAwait(false);
        var accepting = shops.Where(shop => shop.IsAcceptingWork).ToList();

        if (accepting.Count == 0)
        {
            return Result.Failure<SyncReceipt>(Error.Conflict(
                "sync.no_shops",
                "No repair shops are currently accepting work."));
        }

        foreach (var shop in accepting)
        {
            shop.SyncState = ShopSyncState.Pending;
        }

        await repairShopStore.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var ticket = new SyncTicket(
            Guid.NewGuid(),
            accepting.Select(shop => shop.Id).ToList(),
            clock.UtcNow);

        await backlog.EnqueueAsync(ticket, cancellationToken).ConfigureAwait(false);

        auditTrail.Record("SyncRequested", ticket.TicketId.ToString(), $"{accepting.Count} shops queued");

        return Result.Success(new SyncReceipt(ticket.TicketId, accepting.Count));
    }
}
