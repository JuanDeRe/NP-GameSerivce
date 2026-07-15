using GameService.Models;

namespace GameService.Services;

public interface IMatchLifecycleService
{
    Task<int> FinishExpiredMatchesAsync();

    Task<GameSession?> FinishMatchAsync(
        string matchId,
        MatchWinner winner
    );
}