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
}