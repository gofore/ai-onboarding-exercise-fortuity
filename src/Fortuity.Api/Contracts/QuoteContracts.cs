using Fortuity.Domain.Policies;
using Fortuity.Domain.Quotes;

namespace Fortuity.Api.Contracts;

public sealed record DraftQuoteRequest(
    string CustomerNumber,
    PolicyType PolicyType,
    CoverageLevel Coverage,
    int AgeYears,
    int NoClaimsYears);

public sealed record IndicationRequestBody(
    PolicyType PolicyType,
    CoverageLevel Coverage,
    string Region,
    string? Basis,
    int AgeYears,
    int NoClaimsYears);

public sealed record PremiumBreakdownResponse(
    decimal BasePremium,
    decimal RiskMultiplier,
    decimal CoverageMultiplier,
    decimal NoClaimsMultiplier,
    decimal RegionalMultiplier,
    decimal FinalPremium)
{
    internal static PremiumBreakdownResponse From(PremiumBreakdown breakdown) => new(
        breakdown.BasePremium.Amount,
        breakdown.RiskMultiplier,
        breakdown.CoverageMultiplier,
        breakdown.NoClaimsMultiplier,
        breakdown.RegionalMultiplier,
        breakdown.FinalPremium.Amount);
}

public sealed record QuoteResponse(
    int Id,
    int CustomerId,
    string PolicyType,
    string Coverage,
    string Region,
    string BasisUsed,
    int AgeYears,
    int NoClaimsYears,
    PremiumBreakdownResponse Breakdown,
    decimal AnnualPremium,
    DateTimeOffset CreatedAt)
{
    internal static QuoteResponse From(Quote quote) => new(
        quote.Id,
        quote.CustomerId,
        quote.PolicyType.ToString(),
        quote.Coverage.ToString(),
        quote.Region.Value,
        quote.BasisUsed.ToString(),
        quote.VehicleOrPropertyAgeYears,
        quote.NoClaimsYears,
        PremiumBreakdownResponse.From(quote.Breakdown),
        quote.AnnualPremium.Amount,
        quote.CreatedAt);
}

public sealed record IndicationResponse(
    string Region,
    string Basis,
    PremiumBreakdownResponse Breakdown,
    decimal AnnualPremium);
