using GameService.Contracts;

namespace GameService.Services;

public interface ICameraService
{
    ActiveCameraChangedEvent ChangeCamera(
        string matchId,
        ChangeCameraCommand command
    );
}