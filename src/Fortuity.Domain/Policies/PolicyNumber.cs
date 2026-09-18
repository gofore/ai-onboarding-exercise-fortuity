using System.Text.RegularExpressions;

namespace Fortuity.Domain.Policies;

public readonly partial record struct PolicyNumber
{
    private PolicyNumber(string value) => Value = value;

    public string Value { get; }

    public static PolicyNumber From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        var candidate = value.Trim().ToUpperInvariant();
        if (!Pattern().IsMatch(candidate))
        {
            throw new FormatException($"'{value}' is not a valid policy number. Expected POL-yyyy-nnnnnn.");
        }

        return new PolicyNumber(candidate);
    }

    public static PolicyNumber Create(int year, int sequence) =>
        new($"POL-{year:D4}-{sequence:D6}");

    public override string ToString() => Value ?? string.Empty;

    [GeneratedRegex(@"^POL-\d{4}-\d{6}$")]
    private static partial Regex Pattern();
}
