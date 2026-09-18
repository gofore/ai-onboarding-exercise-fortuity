using Fortuity.Domain.Common;

namespace Fortuity.Domain.Claims;

public sealed class Claim
{
    public int Id { get; private set; }

    public required ClaimNumber Number { get; init; }

    public required int PolicyId { get; init; }

    public required ClaimType Type { get; init; }

    public required DateOnly IncidentDate { get; init; }

    public required DateTimeOffset ReportedAt { get; init; }

    public ClaimStatus Status { get; set; } = ClaimStatus.Registered;

    public Money ReserveAmount { get; set; } = Money.Zero;

    public Money PaidAmount { get; set; } = Money.Zero;

    public int? AssignedRepairShopId { get; set; }

    public Money OutstandingReserve => ReserveAmount - PaidAmount;

    public bool IsOpen => Status is not (ClaimStatus.Settled or ClaimStatus.Rejected);
}
