using Domain.Models.Campaign;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Contracts;

public class CampaignConfiguration : IEntityTypeConfiguration<Campaign>
{
    public void Configure(EntityTypeBuilder<Campaign> builder)
    {
        builder.ToTable("campaign");

        builder.HasKey(c => c.CampaignId)
            .HasName("campaign_pkey");

        builder.Property(c => c.CampaignId)
            .HasColumnName("campaign_id");

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.ContractId)
            .HasColumnName("contract_id");

        builder.HasOne(c => c.Contract)
            .WithMany(contract => contract.Campaigns)
            .HasForeignKey(c => c.ContractId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("campaign_contract_fkey");

        builder.HasMany(c => c.Services)
            .WithOne(s => s.Campaign)
            .HasForeignKey(c => c.CampaignId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("campaign_service_fkey");
    }
}
