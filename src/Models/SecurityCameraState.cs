namespace GameService.Models;

/// <summary>
/// Stores the runtime state of a security camera.
/// </summary>
public class SecurityCameraState
{
    public string CameraId { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;

    public bool IsSabotaged { get; set; }
}
