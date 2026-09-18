using Fortuity.Domain.Policies;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuity.Infrastructure.Persistence.Configurations;

internal sealed class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("Policies");
        builder.HasKey(policy => policy.Id);

        builder.Property(policy => policy.Number)
            .HasConversion(ValueConverters.PolicyNumberConverter)
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(policy => policy.Number).IsUnique();

        builder.Property(policy => policy.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(policy => policy.Coverage).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(policy => policy.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.Property(policy => policy.Region)
            .HasConversion(ValueConverters.RegionCodeConverter)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(policy => policy.AnnualPremium)
            .HasConversion(ValueConverters.MoneyConverter)
            .HasPrecision(18, 2);

        builder.HasIndex(policy => policy.CustomerId);
    }
}
