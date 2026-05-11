using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Common;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("address");

        builder.HasKey(a => a.AddressId)
            .HasName("address_pkey");

        builder.Property(a => a.AddressId)
            .HasColumnName("address_id");

        builder.Property(a => a.CountryCode)
            .HasColumnName("country_code")
            .HasMaxLength(3)
            .IsFixedLength();

        builder.Property(a => a.Street)
            .HasColumnName("street")
            .HasMaxLength(100);

        builder.Property(a => a.City)
            .HasColumnName("city")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.State)
            .HasColumnName("state")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(a => a.Country)
            .WithMany()
            .HasForeignKey(a => a.CountryCode)
            .HasConstraintName("address_country_code_fkey");
    }
}
