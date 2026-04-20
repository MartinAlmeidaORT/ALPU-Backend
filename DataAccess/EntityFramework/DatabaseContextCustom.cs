using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.EntityFramework;

public partial class DatabaseContext : DbContext
{
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrator>().HasBaseType<User>();
        modelBuilder.Entity<Supervisor>().HasBaseType<User>();
        modelBuilder.Entity<Accountant>().HasBaseType<User>();
        modelBuilder.Entity<Broadcaster>().HasBaseType<User>();
        modelBuilder.Entity<Client>().HasBaseType<User>();

        modelBuilder.Entity<ServiceDuration>().HasBaseType<Service>();
        modelBuilder.Entity<ServiceSpecial>().HasBaseType<Service>();
        modelBuilder.Entity<ServiceNarrative>().HasBaseType<Service>();
        modelBuilder.Entity<ServiceIVR>().HasBaseType<Service>();
    }
}
