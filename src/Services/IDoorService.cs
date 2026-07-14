using GameService.Contracts;

namespace GameService.Services;

public interface IDoorService
{
    DoorStateChangedEvent SetDoorState(
        string matchId,
        SetDoorStateCommand command
    );
}