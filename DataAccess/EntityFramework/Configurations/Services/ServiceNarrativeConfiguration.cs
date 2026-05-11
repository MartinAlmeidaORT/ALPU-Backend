using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ServiceNarrativeConfiguration : IEntityTypeConfiguration<ServiceNarrative>
{
    public void Configure(EntityTypeBuilder<ServiceNarrative> builder)
    {
        builder.ToTable("service_narrative");

        builder.Property(s => s.BasePrice)
            .HasColumnName("base_price");

        builder.Property(s => s.ExtraPrice)
            .HasColumnName("extra_price");

        builder.Property(s => s.RolPrice)
            .HasColumnName("rol_price");
    }
}
