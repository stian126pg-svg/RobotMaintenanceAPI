using Microsoft.AspNetCore.Mvc;
using RobotMaintenanceApi.Model;
using RobotMaintenanceApi.Services;

namespace RobotMaintenanceApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RobotsController : ControllerBase
{
    private readonly IRobotService _robotService;

    public RobotsController(IRobotService robotService)
    {
        _robotService = robotService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Robot>>> GetRobots(
        string? status = null,
        int page = 1,
        int pageSize = 10)
    {
        IEnumerable<Robot> robots =
            await _robotService.GetAllAsync(status, page, pageSize);

        return Ok(robots);
    }
}