namespace GameService.Models;

public class PlayerState
{
    public string PlayerId { get; set; } = string.Empty;

    public PlayerRole Role { get; set; }
    
    
    public AnimatronicType AnimatronicType { get; set; } = AnimatronicType.None;

    public Position Position { get; set; } = new();

    public string CurrentRoomId { get; set; } = string.Empty;

    public bool IsConnected { get; set; } = true;
}
