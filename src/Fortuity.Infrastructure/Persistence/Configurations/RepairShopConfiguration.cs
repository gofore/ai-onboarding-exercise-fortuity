using Fortuity.Domain.Partners;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuity.Infrastructure.Persistence.Configurations;

internal sealed class RepairShopConfiguration : IEntityTypeConfiguration<RepairShop>
{
    public void Configure(EntityTypeBuilder<RepairShop> builder)
    {
        builder.ToTable("RepairShops");
        builder.HasKey(shop => shop.Id);

        builder.Property(shop => shop.PartnerReference).HasMaxLength(50).IsRequired();
        builder.HasIndex(shop => shop.PartnerReference).IsUnique();

        builder.Property(shop => shop.Name).HasMaxLength(200).IsRequired();
        builder.Property(shop => shop.City).HasMaxLength(100).IsRequired();

        builder.Property(shop => shop.Region)
            .HasConversion(ValueConverters.RegionCodeConverter)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(shop => shop.SyncState).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(shop => shop.LastSyncError).HasMaxLength(500);
    }
}
