using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Fortuity.IntegrationTests;

public class ApiSmokeTests(FortuityApiFactory factory) : IClassFixture<FortuityApiFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private sealed record BreakdownDto(
        decimal BasePremium,
        decimal RiskMultiplier,
        decimal CoverageMultiplier,
        decimal NoClaimsMultiplier,
        decimal RegionalMultiplier,
        decimal FinalPremium);

    private sealed record IndicationDto(string Region, string Basis, BreakdownDto Breakdown, decimal AnnualPremium);

    private sealed record QuoteDto(int Id, string BasisUsed, BreakdownDto Breakdown, decimal AnnualPremium);

    private sealed record ClaimDto(int Id, string Number, string Type, string Status, decimal ReserveAmount);

    private sealed record CustomerDto(string CustomerNumber, string Name, string Region, string PricingBasis);

    [Fact]
    public async Task Health_endpoint_responds()
    {
        var response = await factory.CreateClient().GetAsync(new Uri("/health", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Customer_list_contains_seeded_customers()
    {
        var customers = await factory.CreateClient()
            .GetFromJsonAsync<List<CustomerDto>>(new Uri("/api/customers", UriKind.Relative), JsonOptions);

        Assert.NotNull(customers);
        Assert.Equal(6, customers.Count);
        Assert.Contains(customers, customer => customer.PricingBasis == "Nordic");
    }

    [Fact]
    public async Task Indication_uses_the_requested_basis()
    {
        var client = factory.CreateClient();
        var request = new
        {
            policyType = "Motor",
            coverage = "Standard",
            region = "FI-01",
            basis = "Nordic",
            ageYears = 5,
            noClaimsYears = 5,
        };

        var response = await client.PostAsJsonAsync(new Uri("/api/quotes/indication", UriKind.Relative), request);
        response.EnsureSuccessStatusCode();
        var indication = await response.Content.ReadFromJsonAsync<IndicationDto>(JsonOptions);

        Assert.NotNull(indication);
        Assert.Equal("Nordic", indication.Basis);
        Assert.Equal(1.30m, indication.Breakdown.RegionalMultiplier);
        // 420 × 1.00 × 1.00 × 0.85 × 1.30 = 464.10
        Assert.Equal(464.10m, indication.AnnualPremium);
    }

    [Fact]
    public async Task Indication_falls_back_to_the_local_tariff_when_no_basis_is_given()
    {
        var client = factory.CreateClient();
        var request = new
        {
            policyType = "Motor",
            coverage = "Standard",
            region = "FI-01",
            basis = (string?)null,
            ageYears = 5,
            noClaimsYears = 5,
        };

        var response = await client.PostAsJsonAsync(new Uri("/api/quotes/indication", UriKind.Relative), request);
        response.EnsureSuccessStatusCode();
        var indication = await response.Content.ReadFromJsonAsync<IndicationDto>(JsonOptions);

        Assert.NotNull(indication);
        Assert.Equal("Local", indication.Basis);
        Assert.Equal(1.12m, indication.Breakdown.RegionalMultiplier);
    }

    [Fact]
    public async Task Drafted_quote_rates_on_the_customers_preferred_basis()
    {
        var client = factory.CreateClient();
        var request = new
        {
            customerNumber = "CUST-100001",
            policyType = "Motor",
            coverage = "Standard",
            ageYears = 5,
            noClaimsYears = 5,
        };

        var response = await client.PostAsJsonAsync(new Uri("/api/quotes", UriKind.Relative), request);
        response.EnsureSuccessStatusCode();
        var quote = await response.Content.ReadFromJsonAsync<QuoteDto>(JsonOptions);

        Assert.NotNull(quote);
        Assert.Equal("Local", quote.BasisUsed);
        Assert.Equal(1.12m, quote.Breakdown.RegionalMultiplier);
        // 420 × 1.00 × 1.00 × 0.85 × 1.12 = 399.84
        Assert.Equal(399.84m, quote.AnnualPremium);
    }

    [Fact]
    public async Task Fnol_registers_a_claim_with_a_sequenced_number()
    {
        var client = factory.CreateClient();
        var request = new
        {
            policyNumber = "POL-2026-000004",
            type = "Theft",
            incidentDate = "2026-07-15",
        };

        var response = await client.PostAsJsonAsync(new Uri("/api/claims", UriKind.Relative), request);
        response.EnsureSuccessStatusCode();
        var claim = await response.Content.ReadFromJsonAsync<ClaimDto>(JsonOptions);

        Assert.NotNull(claim);
        Assert.StartsWith("CLM-2026-", claim.Number, StringComparison.Ordinal);
        Assert.Equal("Registered", claim.Status);
        Assert.Equal(1800m, claim.ReserveAmount);
    }
}
