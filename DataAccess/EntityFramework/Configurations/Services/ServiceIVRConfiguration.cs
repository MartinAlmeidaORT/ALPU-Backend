using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ServiceIVRConfiguration : IEntityTypeConfiguration<ServiceIVR>
{
    public void Configure(EntityTypeBuilder<ServiceIVR> builder)
    {
        builder.ToTable("service_ivr");

        builder.Property(s => s.InitialMessagePrice)
            .HasColumnName("initial_message_price");

        builder.Property(s => s.AdditionalMessagePrice)
            .HasColumnName("additional_message_price");

        builder.Property(s => s.UpdateMessagePrice)
            .HasColumnName("update_message_price");

        builder.HasMany(s => s.RangeIVR)
            .WithOne(r => r.Service)
            .HasForeignKey(r => r.ServiceId);
    }
}

public class RangeIVRConfiguration : IEntityTypeConfiguration<RangeIVR>
{
    public void Configure(EntityTypeBuilder<RangeIVR> builder)
    {
        builder.ToTable("range_ivr");

        builder.HasKey(r => new { r.ServiceId, r.MinWord })
            .HasName("range_ivr_pkey");

        builder.Property(r => r.ServiceId)
            .HasColumnName("service_id");

        builder.Property(r => r.MinWord)
            .HasColumnName("min_word");

        builder.Property(r => r.MaxWord)
            .HasColumnName("max_word");

        builder.Property(r => r.PricePerWord)
            .HasColumnName("price_per_word");

        builder.HasOne(r => r.Service)
            .WithMany(s => s.RangeIVR)
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("range_ivr_service_id_fkey");
    }
}
