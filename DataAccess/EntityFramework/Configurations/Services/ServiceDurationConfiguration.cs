using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Services;

public class ServiceDurationConfiguration : IEntityTypeConfiguration<ServiceDuration>
{
    public void Configure(EntityTypeBuilder<ServiceDuration> builder)
    {
        builder.ToTable("service_duration");

        builder.HasMany(sd => sd.ServicePrices)
            .WithOne(sp => sp.Service)
            .HasForeignKey(sp => sp.ServiceId);
    }
}

public class DurationConfiguration : IEntityTypeConfiguration<Duration>
{
    public void Configure(EntityTypeBuilder<Duration> builder)
    {
        builder.ToTable("duration");

        builder.HasKey(d => d.DurationId)
            .HasName("duration_pkey");

        builder.Property(d => d.DurationId)
            .HasColumnName("duration_id");

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.Time)
            .HasColumnName("time");

        builder.HasMany(d => d.ServicePrices)
            .WithOne(sp => sp.Duration)
            .HasForeignKey(sp => sp.DurationId);
    }
}

public class ServicePriceConfiguration : IEntityTypeConfiguration<ServicePrice>
{
    public void Configure(EntityTypeBuilder<ServicePrice> builder)
    {
        builder.ToTable("service_price");

        builder.HasKey(sp => new { sp.ServiceId, sp.DurationId })
            .HasName("service_price_pkey");

        builder.Property(sp => sp.ServiceId)
            .HasColumnName("service_id");

        builder.Property(sp => sp.DurationId)
            .HasColumnName("duration_id");

        builder.Property(sp => sp.Price)
            .HasColumnName("price");

        builder.Property(sp => sp.VariantPrice)
            .HasColumnName("variant_price");

        builder.HasOne(sp => sp.Service)
            .WithMany(s => s.ServicePrices)
            .HasForeignKey(sp => sp.ServiceId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("service_price_service_id_fkey");

        builder.HasOne(sp => sp.Duration)
            .WithMany(d => d.ServicePrices)
            .HasForeignKey(sp => sp.DurationId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("service_price_duration_id_fkey");
    }
}
