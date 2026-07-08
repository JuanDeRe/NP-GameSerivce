using GameService.Contracts;
using GameService.Factories;
using GameService.Models;
using GameService.Stores;

namespace GameService.Services;

public class GameSessionService : IGameSessionService
{
    private const string GameServiceId = "game-service-local";

    private readonly IGameSessionFactory _gameSessionFactory;
    private readonly IGameSessionStore _gameSessionStore;

    public GameSessionService(
        IGameSessionFactory gameSessionFactory,
        IGameSessionStore gameSessionStore)
    {
        _gameSessionFactory = gameSessionFactory;
        _gameSessionStore = gameSessionStore;
    }

    public CreateMatchResponse CreateMatch(CreateMatchRequest request)
    {
        var session = _gameSessionFactory.Create(request);

        _gameSessionStore.Save(session);

        return new CreateMatchResponse
        {
            MatchId = session.MatchId,
            GameServiceId = GameServiceId,
            Status = session.Status.ToString()
        };
    }

    public GameSession? GetMatch(string matchId)
    {
        return _gameSessionStore.GetById(matchId);
    }

    public IReadOnlyCollection<GameSession> GetAllMatches()
    {
        return _gameSessionStore.GetAll();
    }
}