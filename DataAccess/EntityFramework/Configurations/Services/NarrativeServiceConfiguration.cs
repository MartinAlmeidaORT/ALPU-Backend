using Domain.Models.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Services;

public class NarrativeServiceConfiguration : IEntityTypeConfiguration<NarrativeService>
{
    public void Configure(EntityTypeBuilder<NarrativeService> builder)
    {
        builder.ToTable("service");

        builder.Property(s => s.ServiceId)
            .HasColumnName("service_id")
            .ValueGeneratedNever();

        builder.Property(s => s.RolePrice)
            .HasColumnName("role_price");
    }
}
