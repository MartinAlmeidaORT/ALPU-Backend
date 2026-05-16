using Domain.Models.Campaign;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Contracts;

public class CampaignServiceDateConfiguration : IEntityTypeConfiguration<CampaignServiceDate>
{
    public void Configure(EntityTypeBuilder<CampaignServiceDate> builder)
    {
        builder.ToTable("campaign_service_date");

        builder.Property(cs => cs.Date)
            .HasColumnName("date");
    }
}
