using Domain.Models.Campaign;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Contracts;

public class PieceConfiguration : IEntityTypeConfiguration<Piece>
{
    public void Configure(EntityTypeBuilder<Piece> builder)
    {
        builder.ToTable("piece");

        builder.HasKey(p => p.CampaignServiceId)
            .HasName("piece_pkey");

        builder.Property(p => p.CampaignServiceId)
            .HasColumnName("piece_id");

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();
    }
}
