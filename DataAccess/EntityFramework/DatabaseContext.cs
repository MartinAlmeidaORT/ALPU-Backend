using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EntityFramework;

public partial class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Agency> Agencies { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BroadcasterCategory> BroadcasterCategories { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Demo> Demos { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Duration> Durations { get; set; }

    public virtual DbSet<ExtraCharge> ExtraCharges { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Piece> Pieces { get; set; }

    public virtual DbSet<RangeIVR> RangeIvrs { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceDuration> ServiceDurations { get; set; }

    public virtual DbSet<ServiceNarrative> ServiceNarratives { get; set; }

    public virtual DbSet<ServiceSpecial> ServiceSpecials { get; set; }

    public virtual DbSet<ServiceIVR> ServiceIVRs { get; set; }

    public virtual DbSet<ServicePrice> ServicePrices { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Broadcaster> Broadcasters { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Administrator> Admins { get; set; }

    public virtual DbSet<Supervisor> Supervisors { get; set; }

    public virtual DbSet<Accountant> Accountants { get; set; }

    public virtual DbSet<VolumeDiscount> VolumeDiscounts { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //     => optionsBuilder.UseNpgsql("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("bill_type_enum", ["expense", "income"])
            .HasPostgresEnum("membership_state_enum", ["valid", "expired"])
            .HasPostgresEnum("user_state_enum", ["enabled", "pending", "penalized"]);

        modelBuilder.Entity<Accountant>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("address_pkey");

            entity.Property(e => e.CountryCode).IsFixedLength();

            entity.HasOne(d => d.Country).WithMany().HasConstraintName("address_country_code_fkey");
        });

        modelBuilder.Entity<Administrator>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Agency>(entity =>
        {
            entity.HasKey(e => e.AgencyId).HasName("agency_pkey");
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("bill_pkey");

            entity.HasOne(d => d.Contract).WithMany(p => p.Bills).HasConstraintName("bill_contract_id_fkey");
        });

        modelBuilder.Entity<Broadcaster>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();

            entity.HasOne(d => d.Category).WithMany(p => p.Broadcasters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("broadcaster_category_id_fkey");
        });

        modelBuilder.Entity<BroadcasterCategory>(entity =>
        {
            entity.HasKey(e => e.BroadcasterCategoryId).HasName("broadcaster_category_pkey");
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.Property(e => e.UserId).ValueGeneratedNever();

            entity.HasOne(d => d.Agency).WithMany(p => p.Clients)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("client_agency_id_fkey");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("contract_pkey");

            entity.Property(e => e.CountryCode).IsFixedLength();

            entity.HasOne(d => d.Broadcaster).WithMany(p => p.Contracts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contract_broadcaster_id_fkey");

            entity.HasOne(d => d.Client).WithMany(p => p.Contracts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contract_client_id_fkey");

            entity.HasOne(d => d.CountryCodeNavigation).WithMany(p => p.Contracts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("contract_country_code_fkey");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryCode).HasName("country_pkey");

            entity.Property(e => e.CountryCode).IsFixedLength();

            entity.HasOne(d => d.Region).WithMany(p => p.Countries).HasConstraintName("country_region_id_fkey");
        });

        modelBuilder.Entity<Demo>(entity =>
        {
            entity.HasKey(e => new { e.BroadcasterId, e.FileName }).HasName("demo_pkey");

            entity.HasOne(d => d.Broadcaster).WithMany(p => p.Demos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("demo_broadcaster_id_fkey");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("discount_pkey");

            entity.HasMany(d => d.Contracts).WithMany(p => p.Discounts)
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
                        j.HasKey("DiscountId", "ContractId").HasName("discount_contract_pkey");
                        j.ToTable("discount_contract");
                        j.IndexerProperty<int>("DiscountId").HasColumnName("discount_id");
                        j.IndexerProperty<int>("ContractId").HasColumnName("contract_id");
                    });
        });

        modelBuilder.Entity<Duration>(entity =>
        {
            entity.HasKey(e => e.DurationId).HasName("duration_pkey");
        });

        modelBuilder.Entity<ExtraCharge>(entity =>
        {
            entity.HasKey(e => e.ExtraChargeId).HasName("extra_charge_pkey");
        });

        modelBuilder.Entity<ServiceIVR>(entity =>
        {

        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.HasKey(e => e.MembershipId).HasName("membership_pkey");

            entity.HasOne(d => d.Broadcaster).WithMany(p => p.Memberships)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("membership_broadcaster_id_fkey");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("notification_pkey");

            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("notification_user_id_fkey");
        });

        modelBuilder.Entity<Piece>(entity =>
        {
            entity.HasKey(e => e.PieceId).HasName("piece_pkey");

            entity.HasOne(d => d.Contract).WithMany(p => p.Pieces)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("piece_contract_id_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.Pieces)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("piece_service_id_fkey");

            entity.HasMany(d => d.ExtraCharges).WithMany(p => p.Pieces)
                .UsingEntity<Dictionary<string, object>>(
                    "PieceExtraCharge",
                    r => r.HasOne<ExtraCharge>().WithMany()
                        .HasForeignKey("ExtraChargeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("piece_extra_charge_extra_charge_id_fkey"),
                    l => l.HasOne<Piece>().WithMany()
                        .HasForeignKey("PieceId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("piece_extra_charge_piece_id_fkey"),
                    j =>
                    {
                        j.HasKey("PieceId", "ExtraChargeId").HasName("piece_extra_charge_pkey");
                        j.ToTable("piece_extra_charge");
                        j.IndexerProperty<int>("PieceId").HasColumnName("piece_id");
                        j.IndexerProperty<int>("ExtraChargeId").HasColumnName("extra_charge_id");
                    });
        });

        modelBuilder.Entity<RangeIVR>(entity =>
        {
            entity.HasKey(e => new { e.ServiceId, e.MinWord }).HasName("range_ivr_pkey");

            entity.HasOne(d => d.Service).WithMany(p => p.RangeIVR)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("range_ivr_service_id_fkey");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.RegionId).HasName("region_pkey");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("service_pkey");
        });

        modelBuilder.Entity<ServiceDuration>(entity =>
        {

        });

        modelBuilder.Entity<ServiceNarrative>(entity =>
        {

        });

        modelBuilder.Entity<ServicePrice>(entity =>
        {
            entity.HasKey(e => new { e.ServiceId, e.DurationId }).HasName("service_price_pkey");

            entity.HasOne(d => d.Duration).WithMany(p => p.ServicePrices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("service_price_duration_id_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.ServicePrices)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("service_price_service_id_fkey");
        });

        modelBuilder.Entity<ServiceSpecial>(entity =>
        {

        });

        modelBuilder.Entity<Supervisor>(entity =>
        {

        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pkey");

            entity.Property(e => e.GoogleId).HasMaxLength(25).IsRequired(false);

            entity.Property(e => e.UserId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Address).WithOne()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_address_id_fkey");
        });

        modelBuilder.Entity<VolumeDiscount>(entity =>
        {
            entity.HasKey(e => new { e.ServiceId, e.MinQuantity }).HasName("volume_discount_pkey");

            entity.HasOne(d => d.Service).WithMany(p => p.VolumeDiscounts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("volume_discount_service_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
