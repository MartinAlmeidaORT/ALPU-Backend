using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Contracts;

public class PieceConfiguration : IEntityTypeConfiguration<Piece>
{
    public void Configure(EntityTypeBuilder<Piece> builder)
    {
        builder.ToTable("piece");

        builder.HasKey(p => p.PieceId)
            .HasName("piece_pkey");

        builder.Property(p => p.PieceId)
            .HasColumnName("piece_id");

        builder.Property(p => p.ContractId)
            .HasColumnName("contract_id");

        builder.Property(p => p.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.ServiceId)
            .HasColumnName("service_id");

        builder.Property(p => p.Variants)
            .HasColumnName("variants");

        builder.HasOne(p => p.Contract)
            .WithMany(c => c.Pieces)
            .HasForeignKey(p => p.ContractId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("piece_contract_id_fkey");

        builder.HasOne(p => p.Service)
            .WithMany(s => s.Pieces)
            .HasForeignKey(p => p.ServiceId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("piece_service_id_fkey");

        builder.HasMany(p => p.ExtraCharges)
            .WithMany(ec => ec.Pieces)
            .UsingEntity<Dictionary<string, object>>(
                "PieceExtraCharge",
                r => r.HasOne<ExtraCharge>().WithMany()
                    .HasForeignKey("ExtraChargeId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("piece_extra_charge_extra_charge_id_fkey"),
                l => l.HasOne<Piece>().WithMany()
                    .HasForeignKey("PieceId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("piece_extra_charge_piece_id_fkey"),
                j =>
                {
                    j.HasKey("PieceId", "ExtraChargeId")
                        .HasName("piece_extra_charge_pkey");
                    j.ToTable("piece_extra_charge");
                    j.IndexerProperty<int>("PieceId")
                        .HasColumnName("piece_id");
                    j.IndexerProperty<int>("ExtraChargeId")
                        .HasColumnName("extra_charge_id");
                });
    }
}
