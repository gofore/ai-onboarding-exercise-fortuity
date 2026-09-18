using Fortuity.Application.Underwriting;
using Fortuity.Domain.Common;
using Fortuity.Domain.Policies;
using Fortuity.Domain.Underwriting;

namespace Fortuity.UnitTests.Underwriting;

public class RatingEngineTests
{
    private sealed class FixedTariffSource(decimal? multiplier) : ITariffSource
    {
        public Task<decimal?> ResolveMultiplierAsync(
            RegionCode region,
            PolicyType policyType,
            RatingBasis basis,
            DateOnly asOf,
            CancellationToken cancellationToken = default) => Task.FromResult(multiplier);
    }

    private static RatingRequest MotorRequest(int ageYears = 5, int noClaimsYears = 5) => new(
        PolicyType.Motor,
        CoverageLevel.Standard,
        RegionCode.From("FI-01"),
        RatingBasis.Local,
        ageYears,
        noClaimsYears,
        new DateOnly(2026, 8, 1));

    [Fact]
    public async Task Composes_all_multipliers_into_the_final_premium()
    {
        var engine = new RatingEngine(new FixedTariffSource(1.12m));

        var result = await engine.RateAsync(MotorRequest(ageYears: 5, noClaimsYears: 5));

        Assert.True(result.IsSuccess);
        var breakdown = result.Value;
        Assert.Equal(Money.FromEuros(420m), breakdown.BasePremium);
        Assert.Equal(1.00m, breakdown.RiskMultiplier);
        Assert.Equal(1.00m, breakdown.CoverageMultiplier);
        Assert.Equal(0.85m, breakdown.NoClaimsMultiplier);
        Assert.Equal(1.12m, breakdown.RegionalMultiplier);
        // 420 × 1.00 × 1.00 × 0.85 × 1.12 = 399.84
        Assert.Equal(Money.FromEuros(399.84m), breakdown.FinalPremium);
    }

    [Fact]
    public async Task New_vehicles_carry_a_higher_risk_multiplier()
    {
        var engine = new RatingEngine(new FixedTariffSource(1.00m));

        var result = await engine.RateAsync(MotorRequest(ageYears: 2, noClaimsYears: 0));

        Assert.True(result.IsSuccess);
        Assert.Equal(1.15m, result.Value.RiskMultiplier);
    }

    [Fact]
    public async Task No_claims_discount_caps_at_ten_years()
    {
        var engine = new RatingEngine(new FixedTariffSource(1.00m));

        var result = await engine.RateAsync(MotorRequest(noClaimsYears: 25));

        Assert.True(result.IsSuccess);
        Assert.Equal(0.70m, result.Value.NoClaimsMultiplier);
    }

    [Fact]
    public async Task Fails_when_no_tariff_exists()
    {
        var engine = new RatingEngine(new FixedTariffSource(null));

        var result = await engine.RateAsync(MotorRequest());

        Assert.True(result.IsFailure);
        Assert.Equal("tariff.not_found", result.Error.Code);
    }
}
