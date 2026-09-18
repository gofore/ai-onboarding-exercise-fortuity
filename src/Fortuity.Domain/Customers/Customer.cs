using Fortuity.Domain.Common;

namespace Fortuity.Domain.Customers;

public sealed class Customer
{
    public int Id { get; private set; }

    public required string CustomerNumber { get; init; }

    public required string Name { get; init; }

    public required string StreetAddress { get; init; }

    public required string PostalCode { get; init; }

    public required string City { get; init; }

    public required RegionCode Region { get; set; }

    public RatingPreferences Preferences { get; set; } = RatingPreferences.Default;
}
