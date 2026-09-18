using Fortuity.Domain.Claims;
using Fortuity.Domain.Common;
using Fortuity.Domain.Policies;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Fortuity.Infrastructure.Persistence;

internal static class ValueConverters
{
    public static readonly ValueConverter<Money, decimal> MoneyConverter =
        new(money => money.Amount, amount => new Money(amount));

    public static readonly ValueConverter<RegionCode, string> RegionCodeConverter =
        new(region => region.Value, value => RegionCode.From(value));

    public static readonly ValueConverter<PolicyNumber, string> PolicyNumberConverter =
        new(number => number.Value, value => PolicyNumber.From(value));

    public static readonly ValueConverter<ClaimNumber, string> ClaimNumberConverter =
        new(number => number.Value, value => ClaimNumber.From(value));
}
