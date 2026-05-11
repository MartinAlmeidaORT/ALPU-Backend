using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Contracts;

public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder.ToTable("discount");

        builder.HasKey(d => d.DiscountId)
            .HasName("discount_pkey");

        builder.Property(d => d.DiscountId)
            .HasColumnName("discount_id");

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.Amount)
            .HasColumnName("amount");

        builder.HasMany(d => d.Contracts)
            .WithMany(c => c.Discounts)
            .UsingEntity<Dictionary<string, object>>(
                "DiscountContract",
                r => r.HasOne<Contract>().WithMany()
                    .HasForeignKey("ContractId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("discount_contract_contract_id_fkey"),
                l => l.HasOne<Discount>().WithMany()
                    .HasForeignKey("DiscountId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("discount_contract_discount_id_fkey"),
                j =>
                {
                    j.HasKey("DiscountId", "ContractId")
                        .HasName("discount_contract_pkey");
                    j.ToTable("discount_contract");
                    j.IndexerProperty<int>("DiscountId")
                        .HasColumnName("discount_id");
                    j.IndexerProperty<int>("ContractId")
                        .HasColumnName("contract_id");
                });
    }
}
