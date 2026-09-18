using Fortuity.Application.Common;

namespace Fortuity.Infrastructure.Services;

internal sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    public DateOnly Today => DateOnly.FromDateTime(DateTime.Now);
}
