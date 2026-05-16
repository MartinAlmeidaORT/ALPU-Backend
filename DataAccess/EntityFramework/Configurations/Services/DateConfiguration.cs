using Domain.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Services;

public class DateServiceConfiguration : IEntityTypeConfiguration<DateService>
{
    public void Configure(EntityTypeBuilder<DateService> builder)
    {
        builder.ToTable("service");

        builder.Property(s => s.ServiceId)
            .HasColumnName("service_id")
            .ValueGeneratedNever();
    }
}
