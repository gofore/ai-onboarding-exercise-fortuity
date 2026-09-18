namespace Fortuity.Application.PartnerSync;

public interface ISyncBacklog
{
    ValueTask EnqueueAsync(SyncTicket ticket, CancellationToken cancellationToken = default);

    IAsyncEnumerable<SyncTicket> DequeueAllAsync(CancellationToken cancellationToken = default);
}
