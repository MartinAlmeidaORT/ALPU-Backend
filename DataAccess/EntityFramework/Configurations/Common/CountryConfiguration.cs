using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Common;

public class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("country");

        builder.HasKey(c => c.CountryCode)
            .HasName("country_pkey");

        builder.Property(c => c.CountryCode)
            .HasColumnName("country_code")
            .HasMaxLength(3)
            .IsFixedLength();

        builder.Property(c => c.RegionId)
            .HasColumnName("region_id");

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(c => c.Region)
            .WithMany(r => r.Countries)
            .HasForeignKey(c => c.RegionId)
            .HasConstraintName("country_region_id_fkey");

        builder.HasMany(c => c.Contracts)
            .WithOne(c => c.CountryCodeNavigation)
            .HasForeignKey(c => c.CountryCode);
    }
}
