using Fortuity.Domain.Claims;
using Fortuity.Domain.Common;
using Fortuity.Domain.Customers;
using Fortuity.Domain.Partners;
using Fortuity.Domain.Policies;
using Fortuity.Domain.Quotes;
using Fortuity.Domain.Underwriting;
using Microsoft.EntityFrameworkCore;

namespace Fortuity.Infrastructure.Persistence;

public sealed class DatabaseSeeder(FortuityDbContext context)
{
    private static readonly DateOnly OriginalTariff = new(2025, 1, 1);
    private static readonly DateOnly RevisedTariff = new(2026, 7, 1);

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedRegionalFactorsAsync(cancellationToken).ConfigureAwait(false);
        await SeedRepairShopsAsync(cancellationToken).ConfigureAwait(false);
        await SeedCustomersAndPoliciesAsync(cancellationToken).ConfigureAwait(false);
    }

    private async Task SeedRegionalFactorsAsync(CancellationToken cancellationToken)
    {
        if (await context.RegionalFactors.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        // region, motor local/nordic, home local/nordic — original then revised
        (string Region, decimal[] Motor, decimal[] Home)[] tariffs =
        [
            ("FI-01", [1.10m, 1.12m, 1.25m, 1.30m], [1.08m, 1.09m, 1.20m, 1.24m]),
            ("FI-02", [1.00m, 1.02m, 1.15m, 1.18m], [0.98m, 1.00m, 1.12m, 1.15m]),
            ("FI-03", [0.95m, 0.97m, 1.10m, 1.12m], [0.94m, 0.96m, 1.06m, 1.09m]),
            ("FI-19", [0.90m, 0.92m, 1.05m, 1.08m], [0.89m, 0.91m, 1.02m, 1.05m]),
        ];

        foreach (var (region, motor, home) in tariffs)
        {
            context.RegionalFactors.AddRange(
                Factor(region, PolicyType.Motor, RatingBasis.Local, OriginalTariff, motor[0]),
                Factor(region, PolicyType.Motor, RatingBasis.Local, RevisedTariff, motor[1]),
                Factor(region, PolicyType.Motor, RatingBasis.Nordic, OriginalTariff, motor[2]),
                Factor(region, PolicyType.Motor, RatingBasis.Nordic, RevisedTariff, motor[3]),
                Factor(region, PolicyType.Home, RatingBasis.Local, OriginalTariff, home[0]),
                Factor(region, PolicyType.Home, RatingBasis.Local, RevisedTariff, home[1]),
                Factor(region, PolicyType.Home, RatingBasis.Nordic, OriginalTariff, home[2]),
                Factor(region, PolicyType.Home, RatingBasis.Nordic, RevisedTariff, home[3]));
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static RegionalFactor Factor(
        string region,
        PolicyType policyType,
        RatingBasis basis,
        DateOnly effectiveFrom,
        decimal multiplier) => new()
        {
            Region = RegionCode.From(region),
            PolicyType = policyType,
            Basis = basis,
            EffectiveFrom = effectiveFrom,
            Multiplier = multiplier,
        };

    private async Task SeedRepairShopsAsync(CancellationToken cancellationToken)
    {
        if (await context.RepairShops.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        context.RepairShops.AddRange(
            Shop("RN-1001", "Korjaamo Esimerkki Oy", "Helsinki", "FI-01"),
            Shop("RN-1002", "Turun Peltipaja Oy", "Turku", "FI-02"),
            Shop("RN-1003", "Pirkan Autohuolto Oy", "Tampere", "FI-03"),
            Shop("RN-1004", "Pohjolan Korikorjaus Oy", "Oulu", "FI-19"));

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static RepairShop Shop(string reference, string name, string city, string region) => new()
    {
        PartnerReference = reference,
        Name = name,
        City = city,
        Region = RegionCode.From(region),
    };

    private async Task SeedCustomersAndPoliciesAsync(CancellationToken cancellationToken)
    {
        if (await context.Customers.AnyAsync(cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        var customers = new[]
        {
            Customer("CUST-100001", "Aino Virtanen", "Esimerkkikatu 1", "00100", "Helsinki", "FI-01", RatingBasis.Local),
            Customer("CUST-100002", "Eero Lahtinen", "Mallipolku 12", "20100", "Turku", "FI-02", RatingBasis.Local),
            Customer("CUST-100003", "Sanni Koskinen", "Testikuja 7", "33100", "Tampere", "FI-03", RatingBasis.Nordic),
            Customer("CUST-100004", "Mikael Ranta", "Näytetie 3", "90100", "Oulu", "FI-19", RatingBasis.Local),
            Customer("CUST-100005", "Helmi Nieminen", "Esimerkkikatu 44", "02100", "Espoo", "FI-01", RatingBasis.Nordic),
            Customer("CUST-100006", "Onni Salo", "Mallipolku 5", "20200", "Turku", "FI-02", RatingBasis.Local),
        };

        context.Customers.AddRange(customers);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        var sequence = 1;
        var policies = new List<Policy>();
        foreach (var customer in customers)
        {
            policies.Add(new Policy
            {
                Number = PolicyNumber.Create(2026, sequence++),
                CustomerId = customer.Id,
                Type = PolicyType.Motor,
                Coverage = CoverageLevel.Standard,
                Region = customer.Region,
                StartsOn = new DateOnly(2026, 1, 1),
                Status = PolicyStatus.Active,
                AnnualPremium = Money.FromEuros(480m),
            });
        }

        policies.Add(new Policy
        {
            Number = PolicyNumber.Create(2026, sequence++),
            CustomerId = customers[0].Id,
            Type = PolicyType.Home,
            Coverage = CoverageLevel.Plus,
            Region = customers[0].Region,
            StartsOn = new DateOnly(2026, 3, 1),
            Status = PolicyStatus.Active,
            AnnualPremium = Money.FromEuros(310m),
        });

        context.Policies.AddRange(policies);
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        await SeedClaimsAsync(policies, cancellationToken).ConfigureAwait(false);
        await SeedQuotesAsync(customers, cancellationToken).ConfigureAwait(false);
    }

    private async Task SeedQuotesAsync(IReadOnlyList<Customer> customers, CancellationToken cancellationToken)
    {
        context.Quotes.AddRange(
            new Quote
            {
                CustomerId = customers[4].Id, // CUST-100005, Nordic preference
                PolicyType = PolicyType.Motor,
                Coverage = CoverageLevel.Standard,
                Region = customers[4].Region,
                VehicleOrPropertyAgeYears = 5,
                NoClaimsYears = 5,
                BasisUsed = RatingBasis.Nordic,
                CreatedAt = new DateTimeOffset(2026, 7, 10, 10, 30, 0, TimeSpan.FromHours(3)),
                Breakdown = new PremiumBreakdown(
                    Money.FromEuros(420m), 1.00m, 1.00m, 0.85m, 1.30m, Money.FromEuros(464.10m)),
            },
            new Quote
            {
                CustomerId = customers[0].Id, // CUST-100001, Local preference
                PolicyType = PolicyType.Motor,
                Coverage = CoverageLevel.Standard,
                Region = customers[0].Region,
                VehicleOrPropertyAgeYears = 5,
                NoClaimsYears = 5,
                BasisUsed = RatingBasis.Local,
                CreatedAt = new DateTimeOffset(2026, 7, 12, 14, 5, 0, TimeSpan.FromHours(3)),
                Breakdown = new PremiumBreakdown(
                    Money.FromEuros(420m), 1.00m, 1.00m, 0.85m, 1.12m, Money.FromEuros(399.84m)),
            },
            new Quote
            {
                CustomerId = customers[2].Id, // CUST-100003, Nordic preference
                PolicyType = PolicyType.Home,
                Coverage = CoverageLevel.Plus,
                Region = customers[2].Region,
                VehicleOrPropertyAgeYears = 20,
                NoClaimsYears = 8,
                BasisUsed = RatingBasis.Nordic,
                CreatedAt = new DateTimeOffset(2026, 7, 20, 9, 15, 0, TimeSpan.FromHours(3)),
                Breakdown = new PremiumBreakdown(
                    Money.FromEuros(260m), 1.00m, 1.25m, 0.76m, 1.09m, Money.FromEuros(269.23m)),
            });

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static Customer Customer(
        string number,
        string name,
        string street,
        string postalCode,
        string city,
        string region,
        RatingBasis basis) => new()
        {
            CustomerNumber = number,
            Name = name,
            StreetAddress = street,
            PostalCode = postalCode,
            City = city,
            Region = RegionCode.From(region),
            Preferences = new RatingPreferences(basis),
        };

    private async Task SeedClaimsAsync(List<Policy> policies, CancellationToken cancellationToken)
    {
        var shops = await context.RepairShops.ToListAsync(cancellationToken).ConfigureAwait(false);

        context.Claims.AddRange(
            new Claim
            {
                Number = ClaimNumber.Create(2026, 1),
                PolicyId = policies[0].Id,
                Type = ClaimType.Collision,
                IncidentDate = new DateOnly(2026, 2, 14),
                ReportedAt = new DateTimeOffset(2026, 2, 15, 8, 30, 0, TimeSpan.FromHours(2)),
                Status = ClaimStatus.UnderReview,
                ReserveAmount = Money.FromEuros(2500m),
                AssignedRepairShopId = shops[0].Id,
            },
            new Claim
            {
                Number = ClaimNumber.Create(2026, 2),
                PolicyId = policies[1].Id,
                Type = ClaimType.Theft,
                IncidentDate = new DateOnly(2026, 4, 2),
                ReportedAt = new DateTimeOffset(2026, 4, 2, 17, 5, 0, TimeSpan.FromHours(3)),
                Status = ClaimStatus.Approved,
                ReserveAmount = Money.FromEuros(1800m),
                PaidAmount = Money.FromEuros(1200m),
                AssignedRepairShopId = shops[1].Id,
            },
            new Claim
            {
                Number = ClaimNumber.Create(2026, 3),
                PolicyId = policies[2].Id,
                Type = ClaimType.WaterDamage,
                IncidentDate = new DateOnly(2026, 5, 21),
                ReportedAt = new DateTimeOffset(2026, 5, 22, 9, 45, 0, TimeSpan.FromHours(3)),
                Status = ClaimStatus.Settled,
                ReserveAmount = Money.FromEuros(900m),
                PaidAmount = Money.FromEuros(900m),
            });

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
