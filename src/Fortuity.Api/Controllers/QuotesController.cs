using Fortuity.Api.Contracts;
using Fortuity.Application.Underwriting;
using Microsoft.AspNetCore.Mvc;

namespace Fortuity.Api.Controllers;

[Route("api/quotes")]
public sealed class QuotesController(IQuoteDesk quoteDesk, IndicationCalculator indicationCalculator) : FortuityControllerBase
{
    [HttpPost]
    public async Task<ActionResult<QuoteResponse>> Draft(DraftQuoteRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var result = await quoteDesk.DraftAsync(
            new QuoteRequest(
                request.CustomerNumber,
                request.PolicyType,
                request.Coverage,
                request.AgeYears,
                request.NoClaimsYears),
            cancellationToken);

        return result.IsSuccess ? Ok(QuoteResponse.From(result.Value)) : Failure(result.Error);
    }

    [HttpPost("indication")]
    public async Task<ActionResult<IndicationResponse>> Indication(IndicationRequestBody request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var region = RequestMappers.MapRegion(request.Region);
        if (region is null)
        {
            return Problem(title: "indication.invalid_region", detail: "A region code is required.", statusCode: StatusCodes.Status400BadRequest);
        }

        var basis = RequestMappers.MapRatingBasis(request.Basis);

        var result = await indicationCalculator.EstimateAsync(
            new IndicationRequest(
                request.PolicyType,
                request.Coverage,
                region.Value,
                basis,
                request.AgeYears,
                request.NoClaimsYears),
            cancellationToken);

        return result.IsSuccess
            ? Ok(new IndicationResponse(
                region.Value.Value,
                basis.ToString(),
                PremiumBreakdownResponse.From(result.Value),
                result.Value.FinalPremium.Amount))
            : Failure(result.Error);
    }
}
