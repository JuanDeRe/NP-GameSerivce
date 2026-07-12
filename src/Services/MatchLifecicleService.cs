using GameService.Models;
using GameService.Stores;

namespace GameService.Services;

/// <summary>
/// Handles match expiration and finalization.
/// </summary>
public class MatchLifecycleService : IMatchLifecycleService
{
    private readonly IGameSessionStore _gameSessionStore;

    public MatchLifecycleService(IGameSessionStore gameSessionStore)
    {
        _gameSessionStore = gameSessionStore;
    }

    public int FinishExpiredMatches()
    {
        var activeMatches = _gameSessionStore.GetAll()
            .Where(match => match.Status == MatchStatus.Active)
            .ToList();

        var finishedCount = 0;

        foreach (var match in activeMatches)
        {
            lock (match.SyncRoot)
            {
                if (match.StartedAtUtc is null)
                {
                    continue;
                }

                var elapsedSeconds = (DateTime.UtcNow - match.StartedAtUtc.Value).TotalSeconds;

                if (elapsedSeconds < match.DurationSeconds)
                {
                    continue;
                }

                match.Status = MatchStatus.Finished;
                match.Winner = MatchWinner.Guard;
                match.FinishedAtUtc = DateTime.UtcNow;

                _gameSessionStore.Save(match);
                finishedCount++;
            }
        }

        return finishedCount;
    }

    public GameSession? FinishMatch(string matchId, MatchWinner winner)
    {
        var match = _gameSessionStore.GetById(matchId);

        if (match is null)
        {
            return null;
        }

        lock (match.SyncRoot)
        {
            if (match.Status == MatchStatus.Finished)
            {
                return match;
            }

            match.Status = MatchStatus.Finished;
            match.Winner = winner;
            match.FinishedAtUtc = DateTime.UtcNow;

            _gameSessionStore.Save(match);

            return match;
        }
    }
}