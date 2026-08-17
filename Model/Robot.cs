namespace RobotMaintenanceApi.Model;

public class Robot
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public DateTime LastMaintenance { get; set; }
    public DateTime? NextMaintenance { get; set; }
}