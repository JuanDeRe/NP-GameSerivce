namespace GameService.Models;

public class CollectibleSpawnPoint
{
    public string SpawnPointId { get; set; } =
        string.Empty;

    public Position Position { get; set; } = new();

    public string RoomId { get; set; } =
        string.Empty;
}