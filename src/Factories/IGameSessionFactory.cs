using GameService.Contracts;
using GameService.Models;

namespace GameService.Factories;

public interface IGameSessionFactory
{
    GameSession Create(CreateMatchRequest request);
}