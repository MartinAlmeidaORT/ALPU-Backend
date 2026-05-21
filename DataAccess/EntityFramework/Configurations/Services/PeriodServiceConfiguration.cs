using Domain.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Services;

public class PeriodServiceConfiguration : IEntityTypeConfiguration<PeriodService>
{
    public void Configure(EntityTypeBuilder<PeriodService> builder)
    {
        builder.ToTable("service");

        builder.Property(s => s.ServiceId)
            .HasColumnName("service_id")
            .ValueGeneratedNever();

        builder.HasMany(s => s.Periods)
            .WithOne(p => p.Service)
            .HasForeignKey(r => r.ServiceId);

        builder.Navigation(s => s.Periods)
            .AutoInclude();
    }
}

public class PeriodConfiguration : IEntityTypeConfiguration<Period>
{
    public void Configure(EntityTypeBuilder<Period> builder)
    {
        builder.ToTable("service_period");

        builder.HasKey(p => new { p.ServiceId, p.Interval });

        builder.Property(p => p.ServiceId)
            .HasColumnName("service_id");

        builder.Property(p => p.BasePrice)
            .HasColumnName("base_price")
            .IsRequired();

        builder.Property(p => p.ExtraPrice)
            .HasColumnName("extra_price");

        builder.Property(p => p.FirstExtraPrice)
            .HasColumnName("first_extra_price");

        builder.Property(p => p.Interval)
            .HasColumnName("interval")
            .IsRequired();

        builder.HasOne(p => p.Service)
            .WithMany(p => p.Periods)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("service_period_service_id_fkey");
    }
}
