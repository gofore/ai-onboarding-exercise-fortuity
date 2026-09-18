using Fortuity.Domain.Common;

namespace Fortuity.Domain.Policies;

public sealed class Policy
{
    public int Id { get; private set; }

    public required PolicyNumber Number { get; init; }

    public required int CustomerId { get; init; }

    public required PolicyType Type { get; init; }

    public required CoverageLevel Coverage { get; set; }

    public required RegionCode Region { get; set; }

    public required DateOnly StartsOn { get; set; }

    public DateOnly? EndsOn { get; set; }

    public PolicyStatus Status { get; set; } = PolicyStatus.Draft;

    public Money AnnualPremium { get; set; } = Money.Zero;

    public bool IsActiveOn(DateOnly date) =>
        Status == PolicyStatus.Active
        && date >= StartsOn
        && (EndsOn is null || date <= EndsOn);
}
