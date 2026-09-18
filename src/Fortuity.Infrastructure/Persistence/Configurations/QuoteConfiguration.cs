using Fortuity.Domain.Quotes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuity.Infrastructure.Persistence.Configurations;

internal sealed class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.ToTable("Quotes");
        builder.HasKey(quote => quote.Id);

        builder.Ignore(quote => quote.AnnualPremium);

        builder.Property(quote => quote.PolicyType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(quote => quote.Coverage).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(quote => quote.BasisUsed).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.Property(quote => quote.Region)
            .HasConversion(ValueConverters.RegionCodeConverter)
            .HasMaxLength(10)
            .IsRequired();

        builder.OwnsOne(quote => quote.Breakdown, breakdown =>
        {
            breakdown.Property(value => value.BasePremium)
                .HasConversion(ValueConverters.MoneyConverter)
                .HasColumnName("BasePremium")
                .HasPrecision(18, 2);

            breakdown.Property(value => value.RiskMultiplier).HasColumnName("RiskMultiplier").HasPrecision(9, 4);
            breakdown.Property(value => value.CoverageMultiplier).HasColumnName("CoverageMultiplier").HasPrecision(9, 4);
            breakdown.Property(value => value.NoClaimsMultiplier).HasColumnName("NoClaimsMultiplier").HasPrecision(9, 4);
            breakdown.Property(value => value.RegionalMultiplier).HasColumnName("RegionalMultiplier").HasPrecision(9, 4);

            breakdown.Property(value => value.FinalPremium)
                .HasConversion(ValueConverters.MoneyConverter)
                .HasColumnName("FinalPremium")
                .HasPrecision(18, 2);
        });
        builder.Navigation(quote => quote.Breakdown).IsRequired();

        builder.HasIndex(quote => quote.CustomerId);
    }
}
