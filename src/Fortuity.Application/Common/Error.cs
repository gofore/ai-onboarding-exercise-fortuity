namespace Fortuity.Application.Common;

public enum ErrorKind
{
    None = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
}

public sealed record Error(string Code, string Message, ErrorKind Kind)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorKind.None);

    public static Error NotFound(string code, string message) => new(code, message, ErrorKind.NotFound);

    public static Error Validation(string code, string message) => new(code, message, ErrorKind.Validation);

    public static Error Conflict(string code, string message) => new(code, message, ErrorKind.Conflict);
}
