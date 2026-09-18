using Fortuity.Domain.Claims;
using Fortuity.Domain.Common;

namespace Fortuity.UnitTests.Claims;

public class ClaimTests
{
    private static Claim NewClaim() => new()
    {
        Number = ClaimNumber.Create(2026, 42),
        PolicyId = 1,
        Type = ClaimType.Collision,
        IncidentDate = new DateOnly(2026, 3, 14),
        ReportedAt = new DateTimeOffset(2026, 3, 15, 9, 0, 0, TimeSpan.FromHours(2)),
    };

    [Fact]
    public void Outstanding_reserve_is_reserve_less_payments()
    {
        var claim = NewClaim();
        claim.ReserveAmount = Money.FromEuros(2500m);
        claim.PaidAmount = Money.FromEuros(1000m);

        Assert.Equal(Money.FromEuros(1500m), claim.OutstandingReserve);
    }

    [Fact]
    public void Newly_registered_claim_is_open()
    {
        Assert.True(NewClaim().IsOpen);
    }

    [Theory]
    [InlineData(ClaimStatus.Settled)]
    [InlineData(ClaimStatus.Rejected)]
    public void Closed_statuses_are_not_open(ClaimStatus status)
    {
        var claim = NewClaim();
        claim.Status = status;

        Assert.False(claim.IsOpen);
    }
}
