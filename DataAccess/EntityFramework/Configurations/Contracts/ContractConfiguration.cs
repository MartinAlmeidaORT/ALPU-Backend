using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Contracts;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("contract");

        builder.HasKey(c => c.ContractId)
            .HasName("contract_pkey");

        builder.Property(c => c.ContractId)
            .HasColumnName("contract_id");

        builder.Property(c => c.ClientId)
            .HasColumnName("client_id");

        builder.Property(c => c.ClientApproved)
            .HasColumnName("client_approved");

        builder.Property(c => c.BroadcasterId)
            .HasColumnName("broadcaster_id");

        builder.Property(c => c.BroadcasterApproved)
            .HasColumnName("broadcaster_approved");

        builder.Property(c => c.Date)
            .HasColumnName("date");

        builder.Property(c => c.DueDate)
            .HasColumnName("due_date");

        builder.Property(c => c.CountryCode)
            .HasColumnName("country_code")
            .HasMaxLength(3)
            .IsFixedLength();

        builder.Property(c => c.TotalPrice)
            .HasColumnName("total_price");

        builder.Property(c => c.TotalPricePostTax)
            .HasColumnName("total_price_post_tax");

        builder.Property(c => c.State)
            .HasColumnName("state");

        builder.Property(c => c.TermYears)
            .HasColumnName("term_years");

        builder.Property(c => c.PdfAmazonS3Key)
            .HasColumnName("pdf_amazon_s3_key");

        builder.HasOne(c => c.Broadcaster)
            .WithMany(b => b.Contracts)
            .HasForeignKey(c => c.BroadcasterId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("contract_broadcaster_id_fkey");

        builder.HasOne(c => c.Client)
            .WithMany(cl => cl.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("contract_client_id_fkey");

        builder.HasOne(c => c.Country)
            .WithMany(co => co.Contracts)
            .HasForeignKey(c => c.CountryCode)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("contract_country_code_fkey");

        builder.HasMany(c => c.Bills)
            .WithOne(b => b.Contract)
            .HasForeignKey(b => b.ContractId);
    }
}
