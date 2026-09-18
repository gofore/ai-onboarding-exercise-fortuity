using Fortuity.Domain.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuity.Infrastructure.Persistence.Configurations;

internal sealed class ClaimConfiguration : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> builder)
    {
        builder.ToTable("Claims");
        builder.HasKey(claim => claim.Id);

        builder.Ignore(claim => claim.OutstandingReserve);
        builder.Ignore(claim => claim.IsOpen);

        builder.Property(claim => claim.Number)
            .HasConversion(ValueConverters.ClaimNumberConverter)
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(claim => claim.Number).IsUnique();

        builder.Property(claim => claim.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(claim => claim.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

        builder.Property(claim => claim.ReserveAmount)
            .HasConversion(ValueConverters.MoneyConverter)
            .HasPrecision(18, 2);

        builder.Property(claim => claim.PaidAmount)
            .HasConversion(ValueConverters.MoneyConverter)
            .HasPrecision(18, 2);

        builder.HasIndex(claim => claim.PolicyId);
    }
}
