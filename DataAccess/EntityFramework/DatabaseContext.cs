using Domain.Models;
using Domain.Models.Campaign;
using Domain.Models.Services;
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

    public virtual DbSet<PriceAdjustment> PriceAdjustments { get; set; }

    public virtual DbSet<Membership> Memberships { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<BaseService> Services { get; set; }

    public virtual DbSet<IvrService> Ivrs { get; set; }

    public virtual DbSet<RangeIvr> RangeIvrs { get; set; }

    public virtual DbSet<NarrativeService> Narratives { get; set; }

    public virtual DbSet<Piece> Pieces { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Broadcaster> Broadcasters { get; set; }

    public virtual DbSet<Client> Clients { get; set; }

    public virtual DbSet<Administrator> Admins { get; set; }

    public virtual DbSet<Supervisor> Supervisors { get; set; }

    public virtual DbSet<Accountant> Accountants { get; set; }

    public virtual DbSet<VolumeDiscount> VolumeDiscounts { get; set; }

    public virtual DbSet<MultiServiceDiscount> MultiServiceDiscounts { get; set; }

    public virtual DbSet<Campaign> Campaigns { get; set; }

    public virtual DbSet<BaseCampaignService> CampaignServices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Registrar todas las clases del assembly automáticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
    }
}
