using Microsoft.EntityFrameworkCore;
using RobotMaintenanceApi.Data;
using RobotMaintenanceApi.Model;

namespace RobotMaintenanceApi.Services;

public class RobotService : IRobotService
{
    private readonly RobotDbContext _dbContext;

    public RobotService(RobotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Robot>> GetAllAsync(
        string? status,
        int page,
        int pageSize)
    {
        IQueryable<Robot> query =
            _dbContext.Robots.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(status))
        {
            string normalizedStatus =
                status.ToLower();

            query = query.Where(robot =>
                robot.Status.ToLower() == normalizedStatus);
        }

        return await query
            .OrderBy(robot => robot.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Robot?> GetByIdAsync(int id)
    {
        return await _dbContext.Robots
            .AsNoTracking()
            .FirstOrDefaultAsync(robot => robot.Id == id);
    }

    public async Task<Robot> CreateAsync(Robot robot)
    {
        _dbContext.Robots.Add(robot);

        await _dbContext.SaveChangesAsync();

        return robot;
    }
}