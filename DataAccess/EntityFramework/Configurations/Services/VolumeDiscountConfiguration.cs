using Domain.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Services;

public class VolumeDiscountConfiguration : IEntityTypeConfiguration<VolumeDiscount>
{
    public void Configure(EntityTypeBuilder<VolumeDiscount> builder)
    {
        builder.ToTable("volume_discount");

        builder.HasKey(vd => vd.VolumeDiscountId)
            .HasName("volume_discount_pkey");

        builder.Property(vd => vd.VolumeDiscountId)
            .HasColumnName("volume_discount_id");

        builder.Property(vd => vd.ServiceType)
            .HasColumnName("service_type")
            .HasColumnType("service_type_enum")
            .IsRequired();

        builder.Property(vd => vd.MinQuantity)
            .HasColumnName("min_quantity")
            .IsRequired();

        builder.Property(vd => vd.MaxQuantity)
            .HasColumnName("max_quantity");

        builder.Property(vd => vd.Type)
            .HasColumnName("type")
            .HasColumnType("price_adjustment_type_enum")
            .IsRequired();

        builder.Property(vd => vd.Amount)
            .HasColumnName("amount")
            .IsRequired();
    }
}
