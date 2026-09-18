namespace Fortuity.Web.Api;

public sealed record CustomerSummary(
    string CustomerNumber,
    string Name,
    string City,
    string Region,
    string PricingBasis);

public sealed record PolicySummary(
    string Number,
    string Type,
    string Coverage,
    string Region,
    string Status,
    DateOnly StartsOn,
    DateOnly? EndsOn,
    decimal AnnualPremium);

public sealed record CustomerDetail(
    CustomerSummary Customer,
    IReadOnlyList<PolicySummary> Policies);

public sealed record PremiumBreakdownModel(
    decimal BasePremium,
    decimal RiskMultiplier,
    decimal CoverageMultiplier,
    decimal NoClaimsMultiplier,
    decimal RegionalMultiplier,
    decimal FinalPremium);

public sealed record QuoteResult(
    int Id,
    string PolicyType,
    string Coverage,
    string Region,
    string BasisUsed,
    PremiumBreakdownModel Breakdown,
    decimal AnnualPremium,
    DateTimeOffset CreatedAt);

public sealed record IndicationResult(
    string Region,
    string Basis,
    PremiumBreakdownModel Breakdown,
    decimal AnnualPremium);

public sealed record ClaimSummary(
    int Id,
    string Number,
    int PolicyId,
    string Type,
    string Status,
    DateOnly IncidentDate,
    DateTimeOffset ReportedAt,
    decimal ReserveAmount,
    decimal PaidAmount);

public sealed record DraftQuotePayload(
    string CustomerNumber,
    string PolicyType,
    string Coverage,
    int AgeYears,
    int NoClaimsYears);

public sealed record IndicationPayload(
    string PolicyType,
    string Coverage,
    string Region,
    string Basis,
    int AgeYears,
    int NoClaimsYears);

public sealed record FnolPayload(
    string PolicyNumber,
    string Type,
    DateOnly IncidentDate);

public sealed record SyncReceiptModel(Guid TicketId, int ShopCount);

public sealed record RepairShopStatus(
    string PartnerReference,
    string Name,
    string City,
    string Region,
    bool IsAcceptingWork,
    string SyncState,
    DateTimeOffset? LastSyncedAt,
    string? LastSyncError);

public sealed record ApiCall<T>(T? Value, string? Error)
{
    public bool Succeeded => Error is null;
}
