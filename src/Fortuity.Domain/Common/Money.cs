using System.Globalization;

namespace Fortuity.Domain.Common;

public readonly record struct Money : IComparable<Money>
{
    private static readonly CultureInfo DisplayCulture = CultureInfo.GetCultureInfo("fi-FI");

    public Money(decimal amount)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.ToEven);
    }

    public decimal Amount { get; }

    public static Money Zero => new(0m);

    public static Money FromEuros(decimal amount) => new(amount);

    public Money Add(Money other) => new(Amount + other.Amount);

    public Money Subtract(Money other) => new(Amount - other.Amount);

    public Money MultiplyBy(decimal factor) => new(Amount * factor);

    public static Money operator +(Money left, Money right) => left.Add(right);

    public static Money operator -(Money left, Money right) => left.Subtract(right);

    public static Money operator *(Money left, decimal right) => left.MultiplyBy(right);

    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;

    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;

    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;

    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;

    public int CompareTo(Money other) => Amount.CompareTo(other.Amount);

    public override string ToString() => Amount.ToString("C", DisplayCulture);
}
