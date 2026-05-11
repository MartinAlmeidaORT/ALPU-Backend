using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Contracts;

public class VolumeDiscountConfiguration : IEntityTypeConfiguration<VolumeDiscount>
{
    public void Configure(EntityTypeBuilder<VolumeDiscount> builder)
    {
        builder.ToTable("volume_discount");

        builder.HasKey(vd => new { vd.ServiceId, vd.MinQuantity })
            .HasName("volume_discount_pkey");

        builder.Property(vd => vd.ServiceId)
            .HasColumnName("service_id");

        builder.Property(vd => vd.MinQuantity)
            .HasColumnName("min_quantity");

        builder.Property(vd => vd.Discount)
            .HasColumnName("discount");

        builder.HasOne(vd => vd.Service)
            .WithMany(s => s.VolumeDiscounts)
            .HasForeignKey(vd => vd.ServiceId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("volume_discount_service_id_fkey");
    }
}
