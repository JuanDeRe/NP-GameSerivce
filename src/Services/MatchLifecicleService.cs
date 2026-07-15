using GameService.Contracts;
using GameService.Events;
using GameService.Models;
using GameService.Stores;

namespace GameService.Services;

/// <summary>
/// Finishes matches and publishes their final state.
/// </summary>
public class MatchLifecycleService
    : IMatchLifecycleService
{
    private readonly IGameSessionStore _gameSessionStore;
    private readonly IMatchEventPublisher _eventPublisher;

    public MatchLifecycleService(
        IGameSessionStore gameSessionStore,
        IMatchEventPublisher eventPublisher)
    {
        _gameSessionStore = gameSessionStore;
        _eventPublisher = eventPublisher;
    }

    public async Task<int> FinishExpiredMatchesAsync()
    {
        var activeMatches = _gameSessionStore
            .GetAll()
            .Where(match =>
                match.Status == MatchStatus.Active
            )
            .ToList();

        var finishedCount = 0;

        foreach (var match in activeMatches)
        {
            MatchFinishedEvent? matchFinishedEvent = null;

            lock (match.SyncRoot)
            {
                if (match.StartedAtUtc is null)
                {
                    continue;
                }

                var elapsedSeconds =
                    (DateTime.UtcNow -
                     match.StartedAtUtc.Value)
                    .TotalSeconds;

                if (elapsedSeconds <
                    match.DurationSeconds)
                {
                    continue;
                }

                match.Status = MatchStatus.Finished;
                match.Winner = MatchWinner.Guard;
                match.FinishedAtUtc = DateTime.UtcNow;

                _gameSessionStore.Save(match);

                matchFinishedEvent =
                    BuildMatchFinishedEvent(match);

                finishedCount++;
            }

            if (matchFinishedEvent is not null)
            {
                await _eventPublisher
                    .PublishMatchFinishedAsync(
                        matchFinishedEvent
                    );
            }
        }

        return finishedCount;
    }

    public async Task<GameSession?> FinishMatchAsync(
        string matchId,
        MatchWinner winner)
    {
        var match = _gameSessionStore.GetById(matchId);

        if (match is null)
        {
            return null;
        }

        MatchFinishedEvent? matchFinishedEvent = null;

        lock (match.SyncRoot)
        {
            if (match.Status != MatchStatus.Finished)
            {
                match.Status = MatchStatus.Finished;
                match.Winner = winner;
                match.FinishedAtUtc =
                    DateTime.UtcNow;

                _gameSessionStore.Save(match);

                matchFinishedEvent =
                    BuildMatchFinishedEvent(match);
            }
        }

        if (matchFinishedEvent is not null)
        {
            await _eventPublisher
                .PublishMatchFinishedAsync(
                    matchFinishedEvent
                );
        }

        return match;
    }

    private static MatchFinishedEvent
        BuildMatchFinishedEvent(GameSession match)
    {
        return new MatchFinishedEvent
        {
            MatchId = match.MatchId,
            Winner = match.Winner.ToString(),

            PlayerIds = match.Players
                .Select(player => player.PlayerId)
                .ToList(),

            FinishedAtUtc =
                match.FinishedAtUtc ??
                DateTime.UtcNow
        };
    }
}