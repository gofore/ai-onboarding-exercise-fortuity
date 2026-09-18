using Fortuity.Application.Underwriting;
using Fortuity.Domain.Common;
using Fortuity.Domain.Customers;
using Fortuity.Domain.Policies;
using Fortuity.Domain.Underwriting;
using Fortuity.UnitTests.TestDoubles;

namespace Fortuity.UnitTests.Underwriting;

public class QuoteDeskTests
{
    private static Customer NordicCustomer() => new()
    {
        CustomerNumber = "CUST-100005",
        Name = "Helmi Nieminen",
        StreetAddress = "Esimerkkikatu 44",
        PostalCode = "02100",
        City = "Espoo",
        Region = RegionCode.From("FI-01"),
        Preferences = new RatingPreferences(RatingBasis.Nordic),
    };

    private static QuoteDesk BuildDesk(FakeCustomerStore customers, FakeQuoteStore quotes, FakeTariffSource tariffs) =>
        new(customers, quotes, new RatingEngine(tariffs), new QuoteRequestValidator(), new FakeClock());

    [Fact]
    public async Task Rates_on_the_customers_preferred_basis()
    {
        var customers = new FakeCustomerStore();
        customers.Customers.Add(NordicCustomer());
        var quotes = new FakeQuoteStore();
        var tariffs = new FakeTariffSource();
        tariffs.Multipliers[("FI-01", PolicyType.Motor, RatingBasis.Nordic)] = 1.30m;
        tariffs.Multipliers[("FI-01", PolicyType.Motor, RatingBasis.Local)] = 1.12m;

        var result = await BuildDesk(customers, quotes, tariffs)
            .DraftAsync(new QuoteRequest("CUST-100005", PolicyType.Motor, CoverageLevel.Standard, 5, 5));

        Assert.True(result.IsSuccess);
        Assert.Equal(RatingBasis.Nordic, result.Value.BasisUsed);
        Assert.Equal(1.30m, result.Value.Breakdown.RegionalMultiplier);
        Assert.Single(quotes.Added);
    }

    [Fact]
    public async Task Fails_for_an_unknown_customer()
    {
        var desk = BuildDesk(new FakeCustomerStore(), new FakeQuoteStore(), new FakeTariffSource());

        var result = await desk.DraftAsync(new QuoteRequest("CUST-999999", PolicyType.Motor, CoverageLevel.Standard, 5, 5));

        Assert.True(result.IsFailure);
        Assert.Equal("customer.not_found", result.Error.Code);
    }

    [Fact]
    public async Task Rejects_an_invalid_request_before_touching_stores()
    {
        var desk = BuildDesk(new FakeCustomerStore(), new FakeQuoteStore(), new FakeTariffSource());

        var result = await desk.DraftAsync(new QuoteRequest("", PolicyType.Motor, CoverageLevel.Standard, -1, 5));

        Assert.True(result.IsFailure);
        Assert.Equal("quote.invalid_request", result.Error.Code);
    }
}
