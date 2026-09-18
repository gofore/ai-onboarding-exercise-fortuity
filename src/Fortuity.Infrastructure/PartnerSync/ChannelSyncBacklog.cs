using System.Threading.Channels;
using Fortuity.Application.PartnerSync;

namespace Fortuity.Infrastructure.PartnerSync;

internal sealed class ChannelSyncBacklog : ISyncBacklog
{
    private readonly Channel<SyncTicket> _channel = Channel.CreateUnbounded<SyncTicket>();

    public ValueTask EnqueueAsync(SyncTicket ticket, CancellationToken cancellationToken = default) =>
        _channel.Writer.WriteAsync(ticket, cancellationToken);

    public IAsyncEnumerable<SyncTicket> DequeueAllAsync(CancellationToken cancellationToken = default) =>
        _channel.Reader.ReadAllAsync(cancellationToken);
}
