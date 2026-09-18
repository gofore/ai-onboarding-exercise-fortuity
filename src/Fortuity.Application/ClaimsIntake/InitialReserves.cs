using Fortuity.Domain.Claims;
using Fortuity.Domain.Common;

namespace Fortuity.Application.ClaimsIntake;

internal static class InitialReserves
{
    public static Money For(ClaimType type) => type switch
    {
        ClaimType.Collision => Money.FromEuros(2500m),
        ClaimType.Theft => Money.FromEuros(1800m),
        ClaimType.Fire => Money.FromEuros(5000m),
        ClaimType.WaterDamage => Money.FromEuros(1200m),
        _ => Money.FromEuros(1000m),
    };
}
