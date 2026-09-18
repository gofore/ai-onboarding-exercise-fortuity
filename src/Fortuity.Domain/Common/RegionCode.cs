namespace Fortuity.Domain.Common;

public readonly record struct RegionCode
{
    private RegionCode(string value) => Value = value;

    public string Value { get; }

    public static RegionCode From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new RegionCode(value.Trim().ToUpperInvariant());
    }

    public override string ToString() => Value ?? string.Empty;
}
