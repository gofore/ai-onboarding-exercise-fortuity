using Fortuity.Api.Contracts;
using Fortuity.Application.Abstractions;
using Fortuity.Application.ClaimsIntake;
using Fortuity.Application.PartnerSync;
using Microsoft.AspNetCore.Mvc;

namespace Fortuity.Api.Controllers;

[Route("api/claims")]
public sealed class ClaimsController(
    IFnolProcessor fnolProcessor,
    IClaimStore claimStore,
    SyncCoordinator syncCoordinator,
    IRepairShopStore repairShopStore) : FortuityControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ClaimResponse>> Register(FnolRequestBody request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await fnolProcessor.RegisterAsync(
            new FnolReport(request.PolicyNumber, request.Type, request.IncidentDate),
            cancellationToken);

        return result.IsSuccess ? Ok(ClaimResponse.From(result.Value)) : Failure(result.Error);
    }

    [HttpGet("open")]
    public async Task<ActionResult<IReadOnlyList<ClaimResponse>>> ListOpen(CancellationToken cancellationToken)
    {
        var claims = await claimStore.ListOpenAsync(cancellationToken);
        return Ok(claims.Select(ClaimResponse.From).ToList());
    }

    [HttpPost("sync")]
    public async Task<ActionResult<SyncReceiptResponse>> RequestSync(CancellationToken cancellationToken)
    {
        var result = await syncCoordinator.RequestSyncAsync(cancellationToken);

        return result.IsSuccess
            ? Accepted(value: new SyncReceiptResponse(result.Value.TicketId, result.Value.ShopCount))
            : Failure(result.Error);
    }

    [HttpGet("sync/status")]
    public async Task<ActionResult<IReadOnlyList<RepairShopStatusResponse>>> SyncStatus(CancellationToken cancellationToken)
    {
        var shops = await repairShopStore.ListAsync(cancellationToken);
        return Ok(shops.Select(RepairShopStatusResponse.From).ToList());
    }
}
