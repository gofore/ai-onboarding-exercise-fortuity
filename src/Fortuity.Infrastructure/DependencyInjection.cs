using Fortuity.Application.Abstractions;
using Fortuity.Application.Common;
using Fortuity.Application.PartnerSync;
using Fortuity.Application.Underwriting;
using Fortuity.Infrastructure.PartnerSync;
using Fortuity.Infrastructure.Persistence;
using Fortuity.Infrastructure.Services;
using Fortuity.Infrastructure.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fortuity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("Fortuity") ?? "Data Source=fortuity.db";

        services.AddDbContext<FortuityDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<DatabaseSeeder>();

        services.AddScoped<ICustomerStore, EfCustomerStore>();
        services.AddScoped<IPolicyStore, EfPolicyStore>();
        services.AddScoped<IQuoteStore, EfQuoteStore>();
        services.AddScoped<IClaimStore, EfClaimStore>();
        services.AddScoped<ITariffSource, EfTariffSource>();

        services.AddScoped<IRepairShopStore, EfRepairShopStore>();

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IAuditTrail, LoggerAuditTrail>();

        services.AddSingleton<ISyncBacklog, ChannelSyncBacklog>();
        services.AddHostedService<ClaimsSyncWorker>();

        var repairNetBaseUrl = configuration["RepairNet:BaseUrl"] ?? "http://localhost:5015";
        services.AddHttpClient<IRepairNetGateway, RepairNetClient>(client =>
            client.BaseAddress = new Uri(repairNetBaseUrl));

        return services;
    }
}
