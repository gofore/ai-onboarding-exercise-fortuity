using Fortuity.Domain.Common;
using Fortuity.Domain.Underwriting;

namespace Fortuity.Api.Contracts;

internal static class RequestMappers
{
    // Basis is optional for embedded quote widgets; the local tariff is the safe default.
    public static RatingBasis MapRatingBasis(string? basis) => basis switch
    {
        "Nordic" => RatingBasis.Nordic,
        _ => RatingBasis.Local,
    };

    public static RegionCode? MapRegion(string? region)
    {
        if (string.IsNullOrWhiteSpace(region))
        {
            return null;
        }

        return RegionCode.From(region);
    }
}
