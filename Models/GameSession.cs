namespace GameService.Models;

public class GameSession
{
    public string MatchId { get; set; } = string.Empty;

    public MatchStatus Status { get; set; } = MatchStatus.Waiting;

    public List<PlayerState> Players { get; set; } = [];

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? FinishedAtUtc { get; set; }
}