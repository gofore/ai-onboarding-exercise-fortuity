using FluentValidation;
using Fortuity.Application.Abstractions;
using Fortuity.Application.Common;
using Fortuity.Domain.Quotes;

namespace Fortuity.Application.Underwriting;

public sealed class QuoteDesk(
    ICustomerStore customerStore,
    IQuoteStore quoteStore,
    RatingEngine ratingEngine,
    IValidator<QuoteRequest> validator,
    IClock clock) : IQuoteDesk
{
    public async Task<Result<Quote>> DraftAsync(QuoteRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var validation = await validator.ValidateAsync(request, cancellationToken).ConfigureAwait(false);
        if (!validation.IsValid)
        {
            return Result.Failure<Quote>(Error.Validation("quote.invalid_request", validation.ToString("; ")));
        }

        var customer = await customerStore.FindByNumberAsync(request.CustomerNumber, cancellationToken).ConfigureAwait(false);
        if (customer is null)
        {
            return Result.Failure<Quote>(Error.NotFound(
                "customer.not_found",
                $"No customer exists with number {request.CustomerNumber}."));
        }

        var basis = customer.Preferences.PricingBasis;
        var rating = await ratingEngine.RateAsync(
            new RatingRequest(
                request.PolicyType,
                request.Coverage,
                customer.Region,
                basis,
                request.AgeYears,
                request.NoClaimsYears,
                clock.Today),
            cancellationToken).ConfigureAwait(false);

        if (rating.IsFailure)
        {
            return Result.Failure<Quote>(rating.Error);
        }

        var quote = new Quote
        {
            CustomerId = customer.Id,
            PolicyType = request.PolicyType,
            Coverage = request.Coverage,
            Region = customer.Region,
            VehicleOrPropertyAgeYears = request.AgeYears,
            NoClaimsYears = request.NoClaimsYears,
            BasisUsed = basis,
            CreatedAt = clock.UtcNow,
            Breakdown = rating.Value,
        };

        await quoteStore.AddAsync(quote, cancellationToken).ConfigureAwait(false);

        return Result.Success(quote);
    }
}
