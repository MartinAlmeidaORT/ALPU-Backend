using Domain.Models.Campaign;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Contracts;

public class CampaignServiceConfiguration : IEntityTypeConfiguration<BaseCampaignService>
{
    public void Configure(EntityTypeBuilder<BaseCampaignService> builder)
    {
        builder.ToTable("campaign_service");

        builder.UseTptMappingStrategy();

        builder.HasKey(cs => cs.CampaignServiceId)
            .HasName("campaign_service_pkey");

        builder.Property(cs => cs.CampaignServiceId)
            .HasColumnName("campaign_service_id");

        builder.Property(cs => cs.BasePriceOverride)
            .HasColumnName("base_price_override");

        builder.HasOne(cs => cs.Service)
            .WithMany()
            .HasForeignKey(cs => cs.ServiceId)
            .IsRequired();

        builder.HasOne(cs => cs.Campaign)
            .WithMany(c => c.Services)
            .HasForeignKey(cs => cs.CampaignId)
            .IsRequired();

        builder.HasMany(cs => cs.Pieces)
            .WithOne(p => p.CampaignService)
            .HasForeignKey(p => p.CampaignServiceId);
    }
}
