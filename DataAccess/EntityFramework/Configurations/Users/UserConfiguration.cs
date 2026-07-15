using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.EntityFramework.Configurations.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");

        builder.UseTptMappingStrategy();

        builder.HasKey(u => u.UserId)
            .HasName("user_pkey");

        builder.HasIndex(u => u.Email)
            .HasDatabaseName("user_email_key")
            .IsUnique();

        builder.HasIndex(u => u.RUT)
            .HasDatabaseName("user_rut_key")
            .IsUnique();

        builder.HasIndex(u => u.Photo)
            .HasDatabaseName("photo_amazon_s3_key")
            .IsUnique();

        builder.HasIndex(u => u.GoogleId)
            .HasDatabaseName("user_google_id_key")
            .IsUnique();

        builder.Property(u => u.UserId)
            .HasColumnName("user_id")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.GoogleId)
            .HasColumnName("google_id")
            .HasMaxLength(25)
            .IsRequired(false);

        builder.Property(u => u.UserState)
            .HasColumnName("state")
            .HasColumnType("user_state_enum");

        builder.Property(u => u.Email)
            .HasColumnName("email")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Password)
            .HasColumnName("password")
            .HasMaxLength(50);

        builder.Property(u => u.FirstName)
            .HasColumnName("first_name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasColumnName("last_name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.RUT)
            .HasColumnName("rut")
            .HasMaxLength(12)
            .IsRequired();

        builder.Property(u => u.AddressId)
            .HasColumnName("address_id");

        builder.HasOne(u => u.Address)
            .WithOne()
            .HasForeignKey<User>(u => u.AddressId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("user_address_id_fkey");

        builder.HasMany(u => u.Notifications)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AdministratorConfiguration : IEntityTypeConfiguration<Administrator>
{
    public void Configure(EntityTypeBuilder<Administrator> builder)
    {
        builder.ToTable("administrator");

        builder.Property(a => a.UserId)
            .ValueGeneratedNever();
    }
}

public class AccountantConfiguration : IEntityTypeConfiguration<Accountant>
{
    public void Configure(EntityTypeBuilder<Accountant> builder)
    {
        builder.ToTable("accountant");

        builder.Property(a => a.UserId)
            .ValueGeneratedNever();
    }
}

public class SupervisorConfiguration : IEntityTypeConfiguration<Supervisor>
{
    public void Configure(EntityTypeBuilder<Supervisor> builder)
    {
        builder.ToTable("supervisor");

        builder.Property(a => a.UserId)
            .ValueGeneratedNever();
    }
}

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.ToTable("membership");

        builder.HasKey(m => m.MembershipId)
            .HasName("membership_pkey");

        builder.Property(m => m.MembershipId)
            .HasColumnName("membership_id");

        builder.Property(m => m.State)
            .HasColumnName("state")
            .HasColumnType("membership_state_enum");

        builder.Property(m => m.BroadcasterId)
            .HasColumnName("broadcaster_id");

        builder.Property(m => m.PayDate)
            .HasColumnName("pay_date");

        builder.Property(m => m.DueDate)
            .HasColumnName("due_date");

        builder.Property(m => m.Amount)
            .HasColumnName("amount");

        builder.HasOne(m => m.Broadcaster)
            .WithMany(b => b.Memberships)
            .HasForeignKey(m => m.BroadcasterId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("membership_broadcaster_id_fkey");
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notification");

        builder.HasKey(n => n.NotificationId)
            .HasName("notification_pkey");

        builder.Property(n => n.NotificationId)
            .HasColumnName("notification_id");

        builder.Property(n => n.UserId)
            .HasColumnName("user_id");

        builder.Property(n => n.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(n => n.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(n => n.Date)
            .HasColumnName("date")
            .HasColumnType("timestamp without time zone");

        builder.Property(n => n.IsRead)
            .HasColumnName("is_read")
            .HasDefaultValue(false);

        builder.HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("notification_user_id_fkey");
    }
}
