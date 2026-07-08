using GameService.Models;

namespace GameService.Stores;

public interface IGameSessionStore
{
    void Save(GameSession session);

    GameSession? GetById(string matchId);

    IReadOnlyCollection<GameSession> GetAll();
}