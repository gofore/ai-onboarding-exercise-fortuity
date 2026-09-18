using Fortuity.Domain.Common;
using Fortuity.Domain.Policies;
using Fortuity.Domain.Underwriting;

namespace Fortuity.Application.Underwriting;

public sealed record RatingRequest(
    PolicyType PolicyType,
    CoverageLevel Coverage,
    RegionCode Region,
    RatingBasis Basis,
    int AgeYears,
    int NoClaimsYears,
    DateOnly AsOf);
