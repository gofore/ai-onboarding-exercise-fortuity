using Fortuity.Domain.Common;
using Fortuity.Domain.Policies;

namespace Fortuity.UnitTests.Policies;

public class PolicyTests
{
    private static Policy ActivePolicy(DateOnly startsOn, DateOnly? endsOn = null) => new()
    {
        Number = PolicyNumber.Create(2026, 1),
        CustomerId = 1,
        Type = PolicyType.Motor,
        Coverage = CoverageLevel.Standard,
        Region = RegionCode.From("FI-01"),
        StartsOn = startsOn,
        EndsOn = endsOn,
        Status = PolicyStatus.Active,
    };

    [Fact]
    public void Is_active_inside_its_period()
    {
        var policy = ActivePolicy(new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31));

        Assert.True(policy.IsActiveOn(new DateOnly(2026, 6, 1)));
    }

    [Fact]
    public void Is_not_active_before_it_starts()
    {
        var policy = ActivePolicy(new DateOnly(2026, 6, 1));

        Assert.False(policy.IsActiveOn(new DateOnly(2026, 5, 31)));
    }

    [Fact]
    public void Open_ended_policy_stays_active()
    {
        var policy = ActivePolicy(new DateOnly(2026, 1, 1));

        Assert.True(policy.IsActiveOn(new DateOnly(2030, 1, 1)));
    }

    [Fact]
    public void Cancelled_policy_is_never_active()
    {
        var policy = ActivePolicy(new DateOnly(2026, 1, 1));
        policy.Status = PolicyStatus.Cancelled;

        Assert.False(policy.IsActiveOn(new DateOnly(2026, 6, 1)));
    }
}
