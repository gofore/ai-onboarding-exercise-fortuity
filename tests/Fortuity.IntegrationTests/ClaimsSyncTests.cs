using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Fortuity.Application.PartnerSync;
using Fortuity.Domain.Claims;
using Fortuity.Domain.Partners;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Fortuity.IntegrationTests;

public sealed class ClaimsSyncTests(FortuityApiFactory factory) : IClassFixture<FortuityApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private sealed record ShopStatusDto(string PartnerReference, string SyncState, string? LastSyncError);

    private sealed record ReceiptDto(Guid TicketId, int ShopCount);

    private sealed class StubGateway : IRepairNetGateway
    {
        public Task<RepairNetReceipt> PushManifestAsync(
            RepairShop shop,
            IReadOnlyList<Claim> claims,
            CancellationToken cancellationToken = default)
        {
            if (shop.PartnerReference == "RN-1004")
            {
                throw new HttpRequestException("The shop terminal is not reachable.");
            }

            return Task.FromResult(new RepairNetReceipt($"TEST-{shop.PartnerReference}", claims.Count));
        }
    }

    [Fact]
    public async Task Sync_request_flows_through_the_queue_and_worker_to_shop_states()
    {
        using var syncFactory = factory.WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services => services.AddSingleton<IRepairNetGateway>(new StubGateway())));
        var client = syncFactory.CreateClient();

        var response = await client.PostAsync(new Uri("/api/claims/sync", UriKind.Relative), content: null);
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);

        var receipt = await response.Content.ReadFromJsonAsync<ReceiptDto>(JsonOptions);
        Assert.NotNull(receipt);
        Assert.Equal(4, receipt.ShopCount);

        List<ShopStatusDto> shops = [];
        for (var attempt = 0; attempt < 50; attempt++)
        {
            shops = (await client.GetFromJsonAsync<List<ShopStatusDto>>(
                new Uri("/api/claims/sync/status", UriKind.Relative), JsonOptions))!;

            if (shops.TrueForAll(shop => shop.SyncState is not "Pending" and not "NeverSynced"))
            {
                break;
            }

            await Task.Delay(100);
        }

        Assert.Equal(3, shops.Count(shop => shop.SyncState == "Synced"));

        var failed = Assert.Single(shops, shop => shop.SyncState == "Failed");
        Assert.Equal("RN-1004", failed.PartnerReference);
        Assert.Contains("not reachable", failed.LastSyncError, StringComparison.Ordinal);
    }
}
