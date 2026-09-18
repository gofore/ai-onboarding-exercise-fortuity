using Fortuity.Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fortuity.Infrastructure.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(customer => customer.Id);

        builder.Property(customer => customer.CustomerNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(customer => customer.CustomerNumber).IsUnique();

        builder.Property(customer => customer.Name).HasMaxLength(200).IsRequired();
        builder.Property(customer => customer.StreetAddress).HasMaxLength(200).IsRequired();
        builder.Property(customer => customer.PostalCode).HasMaxLength(10).IsRequired();
        builder.Property(customer => customer.City).HasMaxLength(100).IsRequired();

        builder.Property(customer => customer.Region)
            .HasConversion(ValueConverters.RegionCodeConverter)
            .HasMaxLength(10)
            .IsRequired();

        builder.OwnsOne(customer => customer.Preferences, preferences =>
        {
            preferences.Property(value => value.PricingBasis)
                .HasColumnName("PricingBasis")
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();
        });
        builder.Navigation(customer => customer.Preferences).IsRequired();
    }
}
