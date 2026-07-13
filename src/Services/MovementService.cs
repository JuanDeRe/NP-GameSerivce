using GameService.Contracts;
using GameService.Models;
using GameService.Stores;

namespace GameService.Services;

public class MovementService : IMovementService
{
    private const float AnimatronicSpeed = 3f;
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
        lock (session.SyncRoot)
        {
            var currentPosition = player.Position;

            var deltaX = normalizedDirection.X * speed * command.DeltaTime;
            var deltaY = normalizedDirection.Y * speed * command.DeltaTime;

            var candidateX = new Position
            {
                X = currentPosition.X + deltaX,
                Y = currentPosition.Y
            };

            var candidateY = new Position
            {
                X = currentPosition.X,
                Y = currentPosition.Y + deltaY
            };

            var finalPosition = new Position
            {
                X = currentPosition.X,
                Y = currentPosition.Y
            };

            if (_mapService.CanMoveTo(candidateX) &&
                !CollidesWithClosedDoor(session, candidateX))
            {
                finalPosition.X = candidateX.X;
            }

            var candidateFinalY = new Position
            {
                X = finalPosition.X,
                Y = currentPosition.Y + deltaY
            };

            if (_mapService.CanMoveTo(candidateFinalY) &&
                !CollidesWithClosedDoor(session, candidateFinalY))
            {
                finalPosition.Y = candidateFinalY.Y;
            }

            player.Position = finalPosition;
            player.CurrentRoomId = _mapService.GetRoomId(finalPosition);
            
            UpdateAttackZoneState(player);
            

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
    
    private void UpdateAttackZoneState(PlayerState player)
    {
        var map = _mapService.GetCurrentMap();

        var attackZone = map.GameplayZones.FirstOrDefault(zone =>
            zone.Type == MapZoneType.Attack &&
            zone.Contains(player.Position)
        );

        if (attackZone is null)
        {
            player.IsAttacking = false;
            player.ActiveAttackZoneId = null;
            player.AttackProgress = 0f;
            return;
        }

        player.IsAttacking = true;
        player.ActiveAttackZoneId = attackZone.Id;
    }
    private static bool CollidesWithClosedDoor(
        GameSession session,
        Position position)
    {
        return session.Doors.Any(door =>
            door.IsClosed &&
            door.CollisionArea.Contains(position)
        );
    }
}