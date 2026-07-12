using GameService.Models;

namespace GameService.Services;

public interface IMatchLifecycleService
{
    int FinishExpiredMatches();

    GameSession? FinishMatch(string matchId, MatchWinner winner);
}