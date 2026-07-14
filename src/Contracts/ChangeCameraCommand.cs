namespace GameService.Contracts;

public class ChangeCameraCommand
{
    public string PlayerId { get; set; } = string.Empty;

    // Null significa cerrar el monitor.
    public string? CameraId { get; set; }
}