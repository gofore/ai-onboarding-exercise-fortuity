using Fortuity.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Fortuity.Infrastructure.Services;

internal sealed partial class LoggerAuditTrail(ILogger<LoggerAuditTrail> logger) : IAuditTrail
{
    public void Record(string action, string subject, string details) =>
        LogAuditEntry(logger, action, subject, details);

    [LoggerMessage(Level = LogLevel.Information, Message = "AUDIT {Action} [{Subject}] {Details}")]
    private static partial void LogAuditEntry(ILogger logger, string action, string subject, string details);
}
