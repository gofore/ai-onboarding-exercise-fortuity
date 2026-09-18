using Fortuity.Domain.Common;

namespace Fortuity.UnitTests.Common;

public class MoneyTests
{
    [Fact]
    public void Rounds_to_cents_on_construction()
    {
        var money = new Money(10.005m);

        Assert.Equal(10.00m, money.Amount);
    }

    [Fact]
    public void Adds_and_subtracts()
    {
        var sum = Money.FromEuros(100.50m) + Money.FromEuros(49.50m);
        var difference = Money.FromEuros(100m) - Money.FromEuros(40m);

        Assert.Equal(150m, sum.Amount);
        Assert.Equal(60m, difference.Amount);
    }

    [Fact]
    public void Multiplies_by_a_rating_factor()
    {
        var premium = Money.FromEuros(400m).MultiplyBy(1.12m);

        Assert.Equal(448m, premium.Amount);
    }

    [Fact]
    public void Compares_by_amount()
    {
        Assert.True(Money.FromEuros(10m) < Money.FromEuros(20m));
        Assert.True(Money.FromEuros(20m) >= Money.FromEuros(20m));
    }

    [Theory]
    [InlineData(1150, "1 150,00 €")]
    [InlineData(486.5, "486,50 €")]
    [InlineData(1200000, "1 200 000,00 €")]
    public void Formats_amounts_the_Finnish_way(decimal amount, string expected)
    {
        var formatted = Money.FromEuros(amount).ToString();

        // fi-FI groups with a non-breaking space; normalise so the expectation stays readable.
        var normalised = formatted.Replace(' ', ' ').Replace(' ', ' ');

        Assert.Equal(expected, normalised);
    }
}
