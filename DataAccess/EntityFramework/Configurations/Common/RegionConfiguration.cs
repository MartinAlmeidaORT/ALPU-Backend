using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Common;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.ToTable("region");

        builder.HasKey(r => r.RegionId)
            .HasName("region_pkey");

        builder.Property(r => r.RegionId)
            .HasColumnName("region_id");

        builder.Property(r => r.Multiplier)
            .HasColumnName("multiplier");

        builder.HasMany(r => r.Countries)
            .WithOne(c => c.Region)
            .HasForeignKey(c => c.RegionId);
    }
}
