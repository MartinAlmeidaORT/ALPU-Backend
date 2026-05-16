using Domain.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Services;

public class PriceAdjustmentConfiguration : IEntityTypeConfiguration<PriceAdjustment>
{
    public void Configure(EntityTypeBuilder<PriceAdjustment> builder)
    {
        builder.ToTable("price_adjustment");

        builder.HasKey(pa => pa.PriceAdjustmentId)
            .HasName("price_adjustment_pkey");

        builder.Property(pa => pa.PriceAdjustmentId)
            .HasColumnName("price_adjustment_id");

        builder.Property(pa => pa.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(pa => pa.Type)
            .HasColumnName("type")
            .HasColumnType("price_adjustment_type_enum")
            .IsRequired();

        builder.Property(pa => pa.Amount)
            .HasColumnName("amount")
            .IsRequired();

        builder.Property(pa => pa.Key)
            .HasColumnName("key")
            .IsRequired();
    }
}
