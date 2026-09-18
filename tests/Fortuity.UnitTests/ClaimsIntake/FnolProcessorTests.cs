using Fortuity.Application.ClaimsIntake;
using Fortuity.Domain.Claims;
using Fortuity.Domain.Common;
using Fortuity.Domain.Policies;
using Fortuity.UnitTests.TestDoubles;

namespace Fortuity.UnitTests.ClaimsIntake;

public class FnolProcessorTests
{
    private static Policy ActiveMotorPolicy() => new()
    {
        Number = PolicyNumber.Create(2026, 1),
        CustomerId = 1,
        Type = PolicyType.Motor,
        Coverage = CoverageLevel.Standard,
        Region = RegionCode.From("FI-01"),
        StartsOn = new DateOnly(2026, 1, 1),
        Status = PolicyStatus.Active,
    };

    private static FnolProcessor BuildProcessor(FakePolicyStore policies, FakeClaimStore claims) =>
        new(policies, claims, new FnolReportValidator(), new FakeClock());

    [Fact]
    public async Task Registers_a_claim_with_the_initial_reserve_for_its_type()
    {
        var policies = new FakePolicyStore();
        policies.Policies.Add(ActiveMotorPolicy());
        var claims = new FakeClaimStore();

        var result = await BuildProcessor(policies, claims)
            .RegisterAsync(new FnolReport("POL-2026-000001", ClaimType.Collision, new DateOnly(2026, 6, 1)));

        Assert.True(result.IsSuccess);
        Assert.Equal("CLM-2026-000001", result.Value.Number.Value);
        Assert.Equal(Money.FromEuros(2500m), result.Value.ReserveAmount);
        Assert.Equal(ClaimStatus.Registered, result.Value.Status);
        Assert.Single(claims.Added);
    }

    [Fact]
    public async Task Sequences_claim_numbers_within_the_year()
    {
        var policies = new FakePolicyStore();
        policies.Policies.Add(ActiveMotorPolicy());
        var claims = new FakeClaimStore();
        var processor = BuildProcessor(policies, claims);

        await processor.RegisterAsync(new FnolReport("POL-2026-000001", ClaimType.Theft, new DateOnly(2026, 5, 1)));
        var second = await processor.RegisterAsync(new FnolReport("POL-2026-000001", ClaimType.Fire, new DateOnly(2026, 6, 1)));

        Assert.True(second.IsSuccess);
        Assert.Equal("CLM-2026-000002", second.Value.Number.Value);
    }

    [Fact]
    public async Task Rejects_a_claim_on_a_policy_that_was_not_active()
    {
        var policies = new FakePolicyStore();
        var policy = ActiveMotorPolicy();
        policy.Status = PolicyStatus.Cancelled;
        policies.Policies.Add(policy);

        var result = await BuildProcessor(policies, new FakeClaimStore())
            .RegisterAsync(new FnolReport("POL-2026-000001", ClaimType.Collision, new DateOnly(2026, 6, 1)));

        Assert.True(result.IsFailure);
        Assert.Equal("policy.not_active", result.Error.Code);
    }

    [Fact]
    public async Task Rejects_a_malformed_policy_number()
    {
        var result = await BuildProcessor(new FakePolicyStore(), new FakeClaimStore())
            .RegisterAsync(new FnolReport("NOT-A-NUMBER", ClaimType.Collision, new DateOnly(2026, 6, 1)));

        Assert.True(result.IsFailure);
        Assert.Equal("fnol.malformed_policy_number", result.Error.Code);
    }

    [Fact]
    public async Task Audit_decorator_records_registered_claims()
    {
        var policies = new FakePolicyStore();
        policies.Policies.Add(ActiveMotorPolicy());
        var audit = new RecordingAuditTrail();
        var audited = new AuditedFnolProcessor(BuildProcessor(policies, new FakeClaimStore()), audit);

        await audited.RegisterAsync(new FnolReport("POL-2026-000001", ClaimType.Collision, new DateOnly(2026, 6, 1)));

        var entry = Assert.Single(audit.Entries);
        Assert.Equal("ClaimRegistered", entry.Action);
        Assert.Equal("CLM-2026-000001", entry.Subject);
    }
}
