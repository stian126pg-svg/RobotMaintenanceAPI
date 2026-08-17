using RobotMaintenanceApi.Model;

namespace RobotMaintenanceApi.Service;

public interface IRobotService
{
    Task<IEnumerable<Robot>> GetAllSync(
        string? status,
        int page,
        int pageSize);
    
    Task<Robot?> GetByIdAsync(int id);

    Task<Robot> CreateAsync(Robot robot);
}