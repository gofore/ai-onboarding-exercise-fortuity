using System.Net.Http.Json;
using Fortuity.Application.PartnerSync;
using Fortuity.Domain.Claims;
using Fortuity.Domain.Partners;

namespace Fortuity.Infrastructure.PartnerSync;

internal sealed class RepairNetClient(HttpClient http) : IRepairNetGateway
{
    public async Task<RepairNetReceipt> PushManifestAsync(
        RepairShop shop,
        IReadOnlyList<Claim> claims,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(shop);
        ArgumentNullException.ThrowIfNull(claims);

        var manifest = new ManifestPayload(
            shop.PartnerReference,
            claims.Select(claim => new ManifestClaimPayload(
                claim.Number.Value,
                claim.Type.ToString(),
                claim.IncidentDate,
                claim.ReserveAmount.Amount)).ToList());

        var response = await http.PostAsJsonAsync(
            new Uri($"repairnet/v1/shops/{Uri.EscapeDataString(shop.PartnerReference)}/manifest", UriKind.Relative),
            manifest,
            cancellationToken).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();

        var receipt = await response.Content
            .ReadFromJsonAsync<ReceiptPayload>(cancellationToken)
            .ConfigureAwait(false)
            ?? throw new InvalidOperationException("RepairNet returned an empty receipt.");

        return new RepairNetReceipt(receipt.ConfirmationCode, receipt.AcceptedCount);
    }

    private sealed record ManifestPayload(string ShopReference, IReadOnlyList<ManifestClaimPayload> Claims);

    private sealed record ManifestClaimPayload(string Number, string Type, DateOnly IncidentDate, decimal Reserve);

    private sealed record ReceiptPayload(string ConfirmationCode, int AcceptedCount);
}
