using GameService.Models;

namespace GameService.Stores;

public class InMemoryGameSessionStore : IGameSessionStore
{
    private readonly Dictionary<string, GameSession> _sessions = new();

    public void Save(GameSession session)
    {
        _sessions[session.MatchId] = session;
    }

    public GameSession? GetById(string matchId)
    {
        return _sessions.GetValueOrDefault(matchId);
    }

    public IReadOnlyCollection<GameSession> GetAll()
    {
        return _sessions.Values.ToList();
    }
}