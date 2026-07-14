namespace GameService.Models;

/// <summary>
/// Defines a fixed security camera and the rooms it can observe.
/// </summary>
public class SecurityCameraDefinition
{
    public string CameraId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public List<string> VisibleRoomIds { get; set; } = [];

    public bool IsEnabled { get; set; } = true;
}