using Domain.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Services;

public class MultiServiceDiscountConfiguration : IEntityTypeConfiguration<MultiServiceDiscount>
{
    public void Configure(EntityTypeBuilder<MultiServiceDiscount> builder)
    {
        builder.ToTable("multi_service_discount");

        builder.HasKey(m => new { m.ServiceA, m.ServiceB })
            .HasName("multi_service_discount_pkey");

        builder.Property(m => m.Key)
            .HasColumnName("key")
            .IsRequired();

        builder.Property(m => m.ServiceA)
            .HasColumnName("service_a")
            .HasColumnType("service_type_enum");

        builder.Property(m => m.ServiceB)
            .HasColumnName("service_b")
            .HasColumnType("service_type_enum");

        builder.Property(m => m.Type)
            .HasColumnName("type")
            .HasColumnType("price_adjustment_type_enum")
            .IsRequired();

        builder.Property(m => m.Amount)
            .HasColumnName("amount")
            .IsRequired();

        builder.Property(m => m.IsDiscountForServiceBOnly)
            .HasColumnName("is_discount_for_service_b_only")
            .IsRequired();
    }
}
