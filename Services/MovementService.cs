using GameService.Contracts;
using GameService.Models;
using GameService.Stores;

namespace GameService.Services;

public class MovementService : IMovementService
{
    private const float AnimatronicSpeed = 8f;
    private const float GuardSpeed = 0f;

    private readonly IGameSessionStore _gameSessionStore;
    private readonly IMapService _mapService;

    public MovementService(
        IGameSessionStore gameSessionStore,
        IMapService mapService)
    {
        _gameSessionStore = gameSessionStore;
        _mapService = mapService;
    }

    public PlayerMovedEvent Move(string matchId, MoveCommand command)
    {
        var session = _gameSessionStore.GetById(matchId);

        if (session is null)
        {
            throw new InvalidOperationException($"Match '{matchId}' was not found.");
        }

        if (session.Status != MatchStatus.Active)
        {
            throw new InvalidOperationException($"Match '{matchId}' is not active.");
        }

        var player = session.Players.FirstOrDefault(p => p.PlayerId == command.PlayerId);

        if (player is null)
        {
            throw new InvalidOperationException($"Player '{command.PlayerId}' is not part of match '{matchId}'.");
        }

        if (player.Role == PlayerRole.Guard)
        {
            throw new InvalidOperationException("Guard cannot move freely in this prototype.");
        }

        var normalizedDirection = Normalize(command.DirectionX, command.DirectionY);

        var speed = player.Role == PlayerRole.Animatronic
            ? AnimatronicSpeed
            : GuardSpeed;

        var newPosition = new Position
        {
            X = player.Position.X + normalizedDirection.X * speed * command.DeltaTime,
            Y = player.Position.Y + normalizedDirection.Y * speed * command.DeltaTime
        };

        if (!_mapService.CanMoveTo(newPosition))
        {
            newPosition = player.Position;
        }

        player.Position = newPosition;
        player.CurrentRoomId = _mapService.GetRoomId(newPosition);

        _gameSessionStore.Save(session);

        return new PlayerMovedEvent
        {
            MatchId = matchId,
            PlayerId = player.PlayerId,
            X = player.Position.X,
            Y = player.Position.Y,
            RoomId = player.CurrentRoomId
        };
    }

    private static Position Normalize(float x, float y)
    {
        var magnitude = MathF.Sqrt(x * x + y * y);

        if (magnitude == 0)
        {
            return new Position(0, 0);
        }

        return new Position(x / magnitude, y / magnitude);
    }
}