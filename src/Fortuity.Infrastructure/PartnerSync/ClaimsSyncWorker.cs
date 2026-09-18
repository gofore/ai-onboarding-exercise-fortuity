using Fortuity.Application.Abstractions;
using Fortuity.Application.Common;
using Fortuity.Application.PartnerSync;
using Fortuity.Domain.Partners;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fortuity.Infrastructure.PartnerSync;

internal sealed class ClaimsSyncWorker(
    ISyncBacklog backlog,
    IServiceScopeFactory scopeFactory,
    IAuditTrail auditTrail,
    IClock clock) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var ticket in backlog.DequeueAllAsync(stoppingToken).ConfigureAwait(false))
        {
            try
            {
                await ProcessTicketAsync(ticket, stoppingToken).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                // A poisoned ticket must not stop the host (BackgroundServiceExceptionBehavior.StopHost).
                auditTrail.Record("SyncTicketFailed", ticket.TicketId.ToString(), exception.Message);
            }
        }
    }

    private async Task ProcessTicketAsync(SyncTicket ticket, CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var shopStore = scope.ServiceProvider.GetRequiredService<IRepairShopStore>();
        var claimStore = scope.ServiceProvider.GetRequiredService<IClaimStore>();
        var gateway = scope.ServiceProvider.GetRequiredService<IRepairNetGateway>();

        var shops = await shopStore.ListByIdsAsync(ticket.RepairShopIds, cancellationToken).ConfigureAwait(false);

        foreach (var shop in shops)
        {
            await PushShopAsync(shop, claimStore, gateway, cancellationToken).ConfigureAwait(false);
        }

        await shopStore.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        auditTrail.Record("SyncCompleted", ticket.TicketId.ToString(), $"{shops.Count} shops processed");
    }

    private async Task PushShopAsync(
        RepairShop shop,
        IClaimStore claimStore,
        IRepairNetGateway gateway,
        CancellationToken cancellationToken)
    {
        try
        {
            var claims = await claimStore.ListOpenForShopAsync(shop.Id, cancellationToken).ConfigureAwait(false);
            var receipt = await gateway.PushManifestAsync(shop, claims, cancellationToken).ConfigureAwait(false);

            shop.SyncState = ShopSyncState.Synced;
            shop.LastSyncedAt = clock.UtcNow;
            shop.LastSyncError = null;

            auditTrail.Record("ShopSynced", shop.PartnerReference, $"{receipt.AcceptedCount} claims, confirmation {receipt.ConfirmationCode}");
        }
        catch (Exception exception) when (exception is HttpRequestException or InvalidOperationException or TaskCanceledException)
        {
            shop.SyncState = ShopSyncState.Failed;
            shop.LastSyncError = exception.Message;

            auditTrail.Record("ShopSyncFailed", shop.PartnerReference, exception.Message);
        }
    }
}
