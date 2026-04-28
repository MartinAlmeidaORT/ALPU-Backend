using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ExtraChargeConfiguration : IEntityTypeConfiguration<ExtraCharge>
{
    public void Configure(EntityTypeBuilder<ExtraCharge> builder)
    {
        builder.ToTable("extra_charge");

        builder.HasKey(ec => ec.ExtraChargeId)
            .HasName("extra_charge_pkey");

        builder.Property(ec => ec.ExtraChargeId)
            .HasColumnName("extra_charge_id");

        builder.Property(ec => ec.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ec => ec.Amount)
            .HasColumnName("amount");

        builder.HasMany(ec => ec.Pieces)
            .WithMany(p => p.ExtraCharges);
    }
}
