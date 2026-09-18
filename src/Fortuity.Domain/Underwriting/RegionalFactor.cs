using Fortuity.Domain.Common;
using Fortuity.Domain.Policies;

namespace Fortuity.Domain.Underwriting;

public sealed class RegionalFactor
{
    public int Id { get; private set; }

    public required RegionCode Region { get; init; }

    public required PolicyType PolicyType { get; init; }

    public required RatingBasis Basis { get; init; }

    public required DateOnly EffectiveFrom { get; init; }

    public required decimal Multiplier { get; init; }
}
