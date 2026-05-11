using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Services;

public class ServiceSpecialConfiguration : IEntityTypeConfiguration<ServiceSpecial>
{
    public void Configure(EntityTypeBuilder<ServiceSpecial> builder)
    {
        builder.ToTable("service_special");

        builder.Property(s => s.Price)
            .HasColumnName("price");
    }
}
