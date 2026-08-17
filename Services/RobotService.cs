using RobotMaintenanceApi.Model;

namespace RobotMaintenanceApi.Services;

public class RobotService : IRobotService
{
    private readonly List<Robot> _robots =
    [
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
    ];

    public Task<IEnumerable<Robot>> GetAllAsync(
        string? status,
        int page,
        int pageSize)
    {
        IEnumerable<Robot> query = _robots;

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(robot =>
                robot.Status.Equals(
                    status,
                    StringComparison.OrdinalIgnoreCase));
        }

        query = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return Task.FromResult(query);
    }

    public Task<Robot?> GetByIdAsync(int id)
    {
        Robot? robot =
            _robots.FirstOrDefault(robot => robot.Id == id);

        return Task.FromResult(robot);
    }

    public Task<Robot> CreateAsync(Robot robot)
    {
        robot.Id = _robots.Count == 0
            ? 1
            : _robots.Max(existingRobot => existingRobot.Id) + 1;

        _robots.Add(robot);

        return Task.FromResult(robot);
    }
}