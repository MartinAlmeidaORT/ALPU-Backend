using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AgencyConfiguration : IEntityTypeConfiguration<Agency>
{
    public void Configure(EntityTypeBuilder<Agency> builder)
    {
        builder.ToTable("agency");

        builder.HasKey(a => a.AgencyId)
            .HasName("agency_pkey");

        builder.Property(a => a.AgencyId)
            .HasColumnName("agency_id");

        builder.Property(a => a.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.HasMany(a => a.Clients)
            .WithOne(c => c.Agency)
            .HasForeignKey(c => c.AgencyId);
    }
}
