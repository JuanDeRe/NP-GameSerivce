namespace GameService.Models;

public class SpawnPoint
{
    public string Id { get; set; } = string.Empty;

    public PlayerRole Role { get; set; }

    public AnimatronicType AnimatronicType { get; set; }

    public Position Position { get; set; } = new();

    public string RoomId { get; set; } = string.Empty;
}