using RobotMaintenanceApi.Model;

namespace RobotMaintenanceApi.Services;

public interface IRobotService
{
    Task<IEnumerable<Robot>> GetAllAsync(
        string ownerId,
        string? status,
        int page,
        int pageSize);

    Task<Robot?> GetByIdAsync(
        int id,
        string ownerId);

    Task<Robot> CreateAsync(
        Robot robot,
        string ownerId);
}