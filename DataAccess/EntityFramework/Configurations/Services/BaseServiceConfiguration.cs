using CaseConverter;
using Domain.Enums;
using Domain.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Services;

public class BaseServiceConfiguration : IEntityTypeConfiguration<BaseService>
{
    public void Configure(EntityTypeBuilder<BaseService> builder)
    {
        builder.ToTable("service");

        builder.HasDiscriminator(s => s.Discriminator)
               .HasValue<DateService>("date")
               .HasValue<PeriodService>("period")
               .HasValue<IvrService>("ivr")
               .HasValue<NarrativeService>("narrative");

        builder.HasKey(s => s.ServiceId)
            .HasName("service_pkey");

        builder.Property(s => s.ServiceId)
            .HasColumnName("service_id");

        builder.Property(s => s.Discriminator)
            .HasColumnName("discriminator");

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Type)
            .HasColumnName("type")
            .HasColumnType("service_type_enum")
            .IsRequired();

        builder.Property(s => s.BasePrice)
            .HasColumnName("base_price");

        builder.Property(s => s.ExtraPrice)
            .HasColumnName("extra_price");

        builder.Property(s => s.FirstExtraPrice)
            .HasColumnName("first_extra_price");
    }
}
