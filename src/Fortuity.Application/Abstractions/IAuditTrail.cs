namespace Fortuity.Application.Abstractions;

public interface IAuditTrail
{
    void Record(string action, string subject, string details);
}
