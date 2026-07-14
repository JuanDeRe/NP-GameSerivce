using System.Text.Json.Serialization;

namespace GameService.Models;

public class GameSession
{
    public string MatchId { get; set; } = string.Empty;

    public MatchStatus Status { get; set; } = MatchStatus.Waiting;

    public MatchWinner Winner { get; set; } = MatchWinner.None;

    public int DurationSeconds { get; set; }

    public List<PlayerState> Players { get; set; } = [];

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? FinishedAtUtc { get; set; }
    
    public string? ActiveCameraId { get; set; }

    public List<SecurityCameraState> SecurityCameras { get; set; } = [];
    
    public List<DoorState> Doors { get; set; } = [];

    [JsonIgnore]
    public object SyncRoot { get; } = new();
}