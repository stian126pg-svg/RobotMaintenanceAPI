using System.ComponentModel.DataAnnotations;

namespace RobotMaintenanceApi.Dtos;

public class CreateRobotRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;

    [Required]
    public string Status { get; set; } = string.Empty;

    public DateTime LastMaintenance { get; set; }

    public DateTime? NextMaintenance { get; set; }
}