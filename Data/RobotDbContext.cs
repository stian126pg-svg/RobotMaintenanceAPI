using Microsoft.EntityFrameworkCore;
using RobotMaintenanceApi.Model;

namespace RobotMaintenanceApi.Data;

public class RobotDbContext : DbContext
{
    public RobotDbContext(
        DbContextOptions<RobotDbContext> options)
        : base(options)
    {
    }

    public DbSet<Robot> Robots => Set<Robot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Robot>().HasData(
            new Robot
            {
                Id = 1,
                Name = "Atlas",
                Model = "XR-7",
                Status = "Operational",
                LastMaintenance = new DateTime(2026, 8, 1),
                NextMaintenance = new DateTime(2026, 11, 1)
            },
            new Robot
            {
                Id = 2,
                Name = "Hammer",
                Model = "MK-II",
                Status = "NeedsMaintenance",
                LastMaintenance = new DateTime(2026, 4, 15),
                NextMaintenance = new DateTime(2026, 8, 20)
            },
            new Robot
            {
                Id = 3,
                Name = "Bishop",
                Model = "RX-12",
                Status = "Operational",
                LastMaintenance = new DateTime(2026, 7, 10),
                NextMaintenance = new DateTime(2026, 10, 10)
            },
            new Robot
            {
                Id = 4,
                Name = "Rustbucket",
                Model = "MK-I",
                Status = "OutOfService",
                LastMaintenance = new DateTime(2025, 12, 1),
                NextMaintenance = null
            }
        );
    }
}