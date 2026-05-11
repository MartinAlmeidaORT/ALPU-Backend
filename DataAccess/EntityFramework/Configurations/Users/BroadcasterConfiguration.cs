using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Users;

public class BroadcasterConfiguration : IEntityTypeConfiguration<Broadcaster>
{
    public void Configure(EntityTypeBuilder<Broadcaster> builder)
    {
        builder.ToTable("broadcaster");

        builder.UseTptMappingStrategy();

        builder.Property(b => b.UserId)
            .ValueGeneratedNever();

        builder.Property(b => b.CategoryId)
            .HasColumnName("category_id");

        builder.HasOne(b => b.Category)
            .WithMany(c => c.Broadcasters)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("broadcaster_category_id_fkey");

        builder.HasMany(b => b.Contracts)
            .WithOne(c => c.Broadcaster)
            .HasForeignKey(c => c.BroadcasterId);

        builder.HasMany(b => b.Demos)
            .WithOne(d => d.Broadcaster)
            .HasForeignKey(d => d.BroadcasterId);

        builder.HasMany(b => b.Memberships)
            .WithOne(m => m.Broadcaster)
            .HasForeignKey(m => m.BroadcasterId);
    }
}

public class BroadcasterCategoryConfiguration : IEntityTypeConfiguration<BroadcasterCategory>
{
    public void Configure(EntityTypeBuilder<BroadcasterCategory> builder)
    {
        builder.ToTable("broadcaster_category");

        builder.HasKey(bc => bc.BroadcasterCategoryId)
            .HasName("broadcaster_category_pkey");

        builder.Property(bc => bc.BroadcasterCategoryId)
            .HasColumnName("broadcaster_category_id");

        builder.Property(bc => bc.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(bc => bc.LifetimeJobCount)
            .HasColumnName("lifetime_job_count");

        builder.HasMany(bc => bc.Broadcasters)
            .WithOne(b => b.Category)
            .HasForeignKey(b => b.CategoryId);
    }
}

public class DemoConfiguration : IEntityTypeConfiguration<Demo>
{
    public void Configure(EntityTypeBuilder<Demo> builder)
    {
        builder.ToTable("demo");

        builder.HasKey(d => new { d.BroadcasterId, d.FileName })
            .HasName("demo_pkey");

        builder.Property(d => d.BroadcasterId)
            .HasColumnName("broadcaster_id");

        builder.Property(d => d.FileName)
            .HasColumnName("file_name")
            .HasMaxLength(200);

        builder.HasOne(d => d.Broadcaster)
            .WithMany(b => b.Demos)
            .HasForeignKey(d => d.BroadcasterId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("demo_broadcaster_id_fkey");
    }
}
