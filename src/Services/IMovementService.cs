using GameService.Contracts;

namespace GameService.Services;

public interface IMovementService
{
    PlayerMovedEvent Move(string matchId, MoveCommand command);
}