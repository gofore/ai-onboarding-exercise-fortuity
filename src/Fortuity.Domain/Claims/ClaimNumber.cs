using System.Text.RegularExpressions;

namespace Fortuity.Domain.Claims;

public readonly partial record struct ClaimNumber
{
    private ClaimNumber(string value) => Value = value;

    public string Value { get; }

    public static ClaimNumber From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var candidate = value.Trim().ToUpperInvariant();
        if (!Pattern().IsMatch(candidate))
        {
            throw new FormatException($"'{value}' is not a valid claim number. Expected CLM-yyyy-nnnnnn.");
        }

        return new ClaimNumber(candidate);
    }

    public static ClaimNumber Create(int year, int sequence) =>
        new($"CLM-{year:D4}-{sequence:D6}");

    public override string ToString() => Value ?? string.Empty;

    [GeneratedRegex(@"^CLM-\d{4}-\d{6}$")]
    private static partial Regex Pattern();
}
