namespace GameService.Models;

public class MatchCollectible
{
    public string CollectibleId { get; set; } =
        string.Empty;

    public CollectibleType Type { get; set; }

    public string OwnerPlayerId { get; set; } =
        string.Empty;

    public Position Position { get; set; } = new();

    public string RoomId { get; set; } =
        string.Empty;

    public bool IsCollected { get; set; }

    public DateTime? CollectedAtUtc { get; set; }
}