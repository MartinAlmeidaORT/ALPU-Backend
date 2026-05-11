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

    public virtual DbSet<Department> Departments { get; set; }

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("bill_type_enum", ["expense", "income"])
            .HasPostgresEnum("membership_state_enum", ["valid", "expired"])
            .HasPostgresEnum("user_state_enum", ["enabled", "pending", "penalized"]);

        // Registrar todas las clases del assembly automáticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
    }
}
