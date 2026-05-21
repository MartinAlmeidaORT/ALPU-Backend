using Domain.Models;
using Domain.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Services;

public class IvrServiceConfiguration : IEntityTypeConfiguration<IvrService>
{
    public void Configure(EntityTypeBuilder<IvrService> builder)
    {
        builder.ToTable("service");

        builder.Property(s => s.ServiceId)
            .HasColumnName("service_id")
            .ValueGeneratedNever();

        builder.Property(s => s.UpdateMessagePrice)
            .HasColumnName("update_message_price");

        builder.HasMany(s => s.RangeIvr)
            .WithOne(r => r.Service)
            .HasForeignKey(r => r.ServiceId);

        builder.Navigation(s => s.RangeIvr)
            .AutoInclude();
    }
}

public class RangeIvrConfiguration : IEntityTypeConfiguration<RangeIvr>
{
    public void Configure(EntityTypeBuilder<RangeIvr> builder)
    {
        builder.ToTable("range_ivr");

        builder.HasKey(r => new { r.ServiceId, r.MinWord })
            .HasName("range_ivr_pkey");

        builder.Property(r => r.ServiceId)
            .HasColumnName("service_id");

        builder.Property(r => r.MinWord)
            .HasColumnName("min_word")
            .IsRequired();

        builder.Property(r => r.MaxWord)
            .HasColumnName("max_word");

        builder.Property(r => r.PricePerWord)
            .HasColumnName("price_per_word")
            .IsRequired();

        builder.HasOne(r => r.Service)
            .WithMany(s => s.RangeIvr)
            .HasForeignKey(r => r.ServiceId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("range_ivr_service_id_fkey");
    }
}
