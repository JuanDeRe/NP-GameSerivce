using GameService.Contracts;
using GameService.Models;
using GameService.Stores;

namespace GameService.Services;

/// <summary>
/// Validates and updates the guard's active security camera.
/// </summary>
public class CameraService : ICameraService
{
    private readonly IGameSessionStore _gameSessionStore;
    private readonly IMapService _mapService;

    public CameraService(
        IGameSessionStore gameSessionStore,
        IMapService mapService)
    {
        _gameSessionStore = gameSessionStore;
        _mapService = mapService;
    }

    public ActiveCameraChangedEvent ChangeCamera(
        string matchId,
        ChangeCameraCommand command)
    {
        var session = _gameSessionStore.GetById(matchId);

        if (session is null)
        {
            throw new InvalidOperationException(
                $"Match '{matchId}' was not found."
            );
        }

        lock (session.SyncRoot)
        {
            if (session.Status != MatchStatus.Active)
            {
                throw new InvalidOperationException(
                    $"Match '{matchId}' is not active."
                );
            }

            var player = session.Players.FirstOrDefault(
                player => player.PlayerId == command.PlayerId
            );

            if (player is null)
            {
                throw new InvalidOperationException(
                    $"Player '{command.PlayerId}' does not belong to the match."
                );
            }

            if (player.Role != PlayerRole.Guard)
            {
                throw new InvalidOperationException(
                    "Only the guard can use security cameras."
                );
            }

            // Null o vacío significa cerrar el monitor.
            if (string.IsNullOrWhiteSpace(command.CameraId))
            {
                session.ActiveCameraId = null;

                _gameSessionStore.Save(session);

                return BuildEvent(
                    matchId,
                    null,
                    player.PlayerId
                );
            }

            var mapCamera = _mapService
                .GetCurrentMap()
                .SecurityCameras
                .FirstOrDefault(camera =>
                    camera.CameraId == command.CameraId
                );

            if (mapCamera is null)
            {
                throw new InvalidOperationException(
                    $"Camera '{command.CameraId}' was not found."
                );
            }

            var runtimeCamera = session.SecurityCameras
                .FirstOrDefault(camera =>
                    camera.CameraId == command.CameraId
                );

            if (runtimeCamera is null)
            {
                throw new InvalidOperationException(
                    $"Camera state '{command.CameraId}' was not found."
                );
            }

            if (!runtimeCamera.IsEnabled)
            {
                throw new InvalidOperationException(
                    "The selected camera is disabled."
                );
            }

            if (runtimeCamera.IsSabotaged)
            {
                throw new InvalidOperationException(
                    "The selected camera is currently sabotaged."
                );
            }

            session.ActiveCameraId = command.CameraId;

            _gameSessionStore.Save(session);

            return BuildEvent(
                matchId,
                command.CameraId,
                player.PlayerId
            );
        }
    }

    private static ActiveCameraChangedEvent BuildEvent(
        string matchId,
        string? cameraId,
        string playerId)
    {
        return new ActiveCameraChangedEvent
        {
            MatchId = matchId,
            CameraId = cameraId,
            ChangedByPlayerId = playerId
        };
    }
}