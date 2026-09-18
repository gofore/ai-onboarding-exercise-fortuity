using Fortuity.Domain.Underwriting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuity.Infrastructure.Persistence.Configurations;

internal sealed class RegionalFactorConfiguration : IEntityTypeConfiguration<RegionalFactor>
{
    public void Configure(EntityTypeBuilder<RegionalFactor> builder)
    {
        builder.ToTable("RegionalFactors");
        builder.HasKey(factor => factor.Id);

        builder.Property(factor => factor.Region)
            .HasConversion(ValueConverters.RegionCodeConverter)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(factor => factor.PolicyType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(factor => factor.Basis).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(factor => factor.Multiplier).HasPrecision(9, 4).IsRequired();

        builder.HasIndex(factor => new
        {
            factor.Region,
            factor.PolicyType,
            factor.Basis,
            factor.EffectiveFrom,
        }).IsUnique();
    }
}
