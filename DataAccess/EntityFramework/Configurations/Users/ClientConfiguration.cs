using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("client");

        builder.Property(c => c.UserId)
            .ValueGeneratedNever();

        builder.Property(c => c.AgencyId)
            .HasColumnName("agency_id");

        builder.HasOne(c => c.Agency)
            .WithMany(a => a.Clients)
            .HasForeignKey(c => c.AgencyId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("client_agency_id_fkey");

        builder.HasMany(c => c.Contracts)
            .WithOne(c => c.Client)
            .HasForeignKey(c => c.ClientId);
    }
}
