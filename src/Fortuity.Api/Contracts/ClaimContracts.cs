using Fortuity.Domain.Claims;

namespace Fortuity.Api.Contracts;

public sealed record FnolRequestBody(
    string PolicyNumber,
    ClaimType Type,
    DateOnly IncidentDate);

public sealed record ClaimResponse(
    int Id,
    string Number,
    int PolicyId,
    string Type,
    string Status,
    DateOnly IncidentDate,
    DateTimeOffset ReportedAt,
    decimal ReserveAmount,
    decimal PaidAmount,
    int? AssignedRepairShopId)
{
    internal static ClaimResponse From(Claim claim) => new(
        claim.Id,
        claim.Number.Value,
        claim.PolicyId,
        claim.Type.ToString(),
        claim.Status.ToString(),
        claim.IncidentDate,
        claim.ReportedAt,
        claim.ReserveAmount.Amount,
        claim.PaidAmount.Amount,
        claim.AssignedRepairShopId);
}
