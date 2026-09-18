using Fortuity.Application.PartnerSync;
using Fortuity.Domain.Common;
using Fortuity.Domain.Partners;
using Fortuity.UnitTests.TestDoubles;

namespace Fortuity.UnitTests.PartnerSync;

public class SyncCoordinatorTests
{
    private static RepairShop Shop(int id, string reference, bool accepting = true)
    {
        var shop = new RepairShop
        {
            PartnerReference = reference,
            Name = $"Shop {reference}",
            City = "Turku",
            Region = RegionCode.From("FI-02"),
            IsAcceptingWork = accepting,
        };

        typeof(RepairShop).GetProperty(nameof(RepairShop.Id))!.SetValue(shop, id);
        return shop;
    }

    [Fact]
    public async Task Queues_a_ticket_for_shops_accepting_work_and_marks_them_pending()
    {
        var store = new FakeRepairShopStore();
        store.Shops.Add(Shop(1, "RN-1001"));
        store.Shops.Add(Shop(2, "RN-1002"));
        store.Shops.Add(Shop(3, "RN-1003", accepting: false));
        var backlog = new FakeSyncBacklog();
        var audit = new RecordingAuditTrail();

        var result = await new SyncCoordinator(store, backlog, audit, new FakeClock()).RequestSyncAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.ShopCount);

        var ticket = Assert.Single(backlog.Enqueued);
        Assert.Equal([1, 2], ticket.RepairShopIds);

        Assert.Equal(ShopSyncState.Pending, store.Shops[0].SyncState);
        Assert.Equal(ShopSyncState.Pending, store.Shops[1].SyncState);
        Assert.Equal(ShopSyncState.NeverSynced, store.Shops[2].SyncState);

        var entry = Assert.Single(audit.Entries);
        Assert.Equal("SyncRequested", entry.Action);
    }

    [Fact]
    public async Task Fails_when_no_shop_accepts_work()
    {
        var store = new FakeRepairShopStore();
        store.Shops.Add(Shop(1, "RN-1001", accepting: false));
        var backlog = new FakeSyncBacklog();

        var result = await new SyncCoordinator(store, backlog, new RecordingAuditTrail(), new FakeClock()).RequestSyncAsync();

        Assert.True(result.IsFailure);
        Assert.Equal("sync.no_shops", result.Error.Code);
        Assert.Empty(backlog.Enqueued);
    }
}
