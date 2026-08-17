using Microsoft.AspNetCore.Mvc;
using RobotMaintenanceApi.Dtos;
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
    [ProducesResponseType(
        typeof(IEnumerable<Robot>),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<Robot>>> GetRobots(
        string? status = null,
        int page = 1,
        int pageSize = 10)
    {
        if (page < 1)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid page number",
                detail: "Page must be greater than or equal to 1.");
        }

        if (pageSize < 1)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid page size",
                detail: "Page size must be greater than or equal to 1.");
        }

        IEnumerable<Robot> robots =
            await _robotService.GetAllAsync(
                status,
                page,
                pageSize);

        return Ok(robots);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(
        typeof(Robot),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Robot>> GetRobot(int id)
    {
        Robot? robot =
            await _robotService.GetByIdAsync(id);

        if (robot is null)
        {
            return NotFound();
        }

        return Ok(robot);
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(Robot),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Robot>> CreateRobot(
        CreateRobotRequest request)
    {
        string[] validStatuses =
        [
            "Operational",
            "NeedsMaintenance",
            "OutOfService"
        ];

        if (!validStatuses.Contains(
            request.Status,
            StringComparer.OrdinalIgnoreCase))
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Invalid robot status",
                detail:
                    "Status must be Operational, NeedsMaintenance, or OutOfService.");
        }

        Robot robot = new()
        {
            Name = request.Name,
            Model = request.Model,
            Status = request.Status,
            LastMaintenance = request.LastMaintenance,
            NextMaintenance = request.NextMaintenance
        };

        Robot createdRobot =
            await _robotService.CreateAsync(robot);

        return CreatedAtAction(
            nameof(GetRobot),
            new { id = createdRobot.Id },
            createdRobot);
    }
}