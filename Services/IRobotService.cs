using RobotMaintenanceApi.Model;

namespace RobotMaintenanceApi.Services;

public interface IRobotService
{
    Task<IEnumerable<Robot>> GetAllAsync(
        string? status,
        int page,
        int pageSize);

    Task<Robot?> GetByIdAsync(int id);

    Task<Robot> CreateAsync(Robot robot);
}