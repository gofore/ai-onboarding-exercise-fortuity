using FluentValidation;
using Fortuity.Application.Abstractions;
using Fortuity.Application.ClaimsIntake;
using Fortuity.Application.PartnerSync;
using Fortuity.Application.Underwriting;
using Microsoft.Extensions.DependencyInjection;

namespace Fortuity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddScoped<RatingEngine>();
        services.AddScoped<IndicationCalculator>();
        services.AddScoped<SyncCoordinator>();

        services.AddScoped<QuoteDesk>();
        services.AddScoped<IQuoteDesk>(provider => new AuditedQuoteDesk(
            provider.GetRequiredService<QuoteDesk>(),
            provider.GetRequiredService<IAuditTrail>()));

        services.AddScoped<FnolProcessor>();
        services.AddScoped<IFnolProcessor>(provider => new AuditedFnolProcessor(
            provider.GetRequiredService<FnolProcessor>(),
            provider.GetRequiredService<IAuditTrail>()));

        return services;
    }
}
