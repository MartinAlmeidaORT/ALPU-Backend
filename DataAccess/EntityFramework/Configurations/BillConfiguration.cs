using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations;

public class BillConfiguration : IEntityTypeConfiguration<Bill>
{
    public void Configure(EntityTypeBuilder<Bill> builder)
    {
        builder.ToTable("bill");

        builder.HasKey(b => b.BillId)
            .HasName("bill_pkey");

        builder.Property(b => b.BillId)
            .HasColumnName("bill_id");

        builder.Property(b => b.Type)
            .HasColumnName("type")
            .HasColumnType("bill_type_enum");

        builder.Property(b => b.ContractId)
            .HasColumnName("contract_id");

        builder.Property(b => b.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(b => b.Date)
            .HasColumnName("date");

        builder.Property(b => b.Amount)
            .HasColumnName("amount");

        builder.Property(b => b.ProofAmazonS3Key)
            .HasColumnName("proof_amazon_s3_key")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(b => b.Contract)
            .WithMany(c => c.Bills)
            .HasForeignKey(b => b.ContractId)
            .HasConstraintName("bill_contract_id_fkey");
    }
}
