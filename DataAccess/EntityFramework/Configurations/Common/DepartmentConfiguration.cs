using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Common;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("department");

        builder.HasKey(d => d.DepartmentId)
            .HasName("department_pkey");

        builder.Property(d => d.DepartmentId)
            .HasColumnName("department_id");

        builder.Property(d => d.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();

        builder.HasOne(d => d.Country)
            .WithMany(c => c.Departments)
            .HasForeignKey(d => d.CountryCode)
            .HasConstraintName("department_country_code_fkey");
    }
}
