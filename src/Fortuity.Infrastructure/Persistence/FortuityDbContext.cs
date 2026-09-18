using Fortuity.Domain.Claims;
using Fortuity.Domain.Customers;
using Fortuity.Domain.Partners;
using Fortuity.Domain.Policies;
using Fortuity.Domain.Quotes;
using Fortuity.Domain.Underwriting;
using Microsoft.EntityFrameworkCore;

namespace Fortuity.Infrastructure.Persistence;

public sealed class FortuityDbContext(DbContextOptions<FortuityDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Policy> Policies => Set<Policy>();

    public DbSet<Quote> Quotes => Set<Quote>();

    public DbSet<Claim> Claims => Set<Claim>();

    public DbSet<RepairShop> RepairShops => Set<RepairShop>();

    public DbSet<RegionalFactor> RegionalFactors => Set<RegionalFactor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FortuityDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
