using Microsoft.AspNetCore.Mvc;

namespace Fortuity.Api.Controllers;

// Stand-in for the external RepairNet partner API until the production
// endpoint contract is finalised with the network operator.
[ApiController]
[Route("repairnet/v1")]
public sealed class RepairNetStubController : ControllerBase
{
    public sealed record ManifestBody(string ShopReference, IReadOnlyList<ManifestClaim> Claims);

    public sealed record ManifestClaim(string Number, string Type, DateOnly IncidentDate, decimal Reserve);

    public sealed record ManifestReceipt(string ConfirmationCode, int AcceptedCount);

    [HttpPost("shops/{reference}/manifest")]
    public ActionResult<ManifestReceipt> AcceptManifest(string reference, ManifestBody body)
    {
        ArgumentNullException.ThrowIfNull(body);

        // RN-1004's terminal has been offline since the Oulu network migration.
        if (reference == "RN-1004")
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new ProblemDetails { Title = "shop_terminal_offline", Detail = "The shop terminal is not reachable." });
        }

        return Ok(new ManifestReceipt(
            $"RN-{reference}-{body.Claims.Count:D3}",
            body.Claims.Count));
    }
}
