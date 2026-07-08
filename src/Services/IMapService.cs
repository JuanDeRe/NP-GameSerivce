using GameService.Models;

namespace GameService.Services;

public interface IMapService
{
    GameMap GetCurrentMap();

    string GetRoomId(Position position);

    bool CanMoveTo(Position position);
}