using Fortuity.Domain.Policies;

namespace Fortuity.UnitTests.Policies;

public class PolicyNumberTests
{
    [Fact]
    public void Creates_a_padded_number_from_year_and_sequence()
    {
        var number = PolicyNumber.Create(2026, 123);

        Assert.Equal("POL-2026-000123", number.Value);
    }

    [Fact]
    public void Parses_a_well_formed_number()
    {
        var number = PolicyNumber.From("pol-2026-000123");

        Assert.Equal("POL-2026-000123", number.Value);
    }

    [Theory]
    [InlineData("CLM-2026-000123")]
    [InlineData("POL-26-000123")]
    [InlineData("POL-2026-123")]
    public void Rejects_malformed_numbers(string candidate)
    {
        Assert.Throws<FormatException>(() => PolicyNumber.From(candidate));
    }
}
