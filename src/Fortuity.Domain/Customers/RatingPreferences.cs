using Fortuity.Domain.Underwriting;

namespace Fortuity.Domain.Customers;

public sealed record RatingPreferences(RatingBasis PricingBasis)
{
    public static RatingPreferences Default { get; } = new(RatingBasis.Local);
}
