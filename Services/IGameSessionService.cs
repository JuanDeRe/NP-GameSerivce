using GameService.Contracts;
using GameService.Models;

namespace GameService.Services;

public interface IGameSessionService
{
    CreateMatchResponse CreateMatch(CreateMatchRequest request);

    GameSession? GetMatch(string matchId);

    IReadOnlyCollection<GameSession> GetAllMatches();
}