using System.ComponentModel.DataAnnotations;

namespace RobotMaintenanceApi.Model;

public class Robot
{
    public int Id { get; set; }

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