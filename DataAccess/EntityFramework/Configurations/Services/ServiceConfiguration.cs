using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("service");

        builder.UseTptMappingStrategy();

        builder.HasKey(s => s.ServiceId)
            .HasName("service_pkey");

        builder.Property(s => s.ServiceId)
            .HasColumnName("service_id");

        builder.Property(s => s.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasMany(s => s.Pieces)
            .WithOne(p => p.Service)
            .HasForeignKey(p => p.ServiceId);

        builder.HasMany(s => s.VolumeDiscounts)
            .WithOne(vd => vd.Service)
            .HasForeignKey(vd => vd.ServiceId);
    }
}
