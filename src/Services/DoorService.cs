using GameService.Contracts;
using GameService.Models;
using GameService.Stores;

namespace GameService.Services;

/// <summary>
/// Validates and updates the authoritative state of security doors.
/// </summary>
public class DoorService : IDoorService
{
    private readonly IGameSessionStore _gameSessionStore;

    public DoorService(
        IGameSessionStore gameSessionStore)
    {
        _gameSessionStore = gameSessionStore;
    }

    public DoorStateChangedEvent SetDoorState(
        string matchId,
        SetDoorStateCommand command)
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
                    "Only the guard can operate security doors."
                );
            }

            var door = session.Doors.FirstOrDefault(
                door => door.DoorId == command.DoorId
            );

            if (door is null)
            {
                throw new InvalidOperationException(
                    $"Door '{command.DoorId}' was not found."
                );
            }
            
            if (command.IsClosed)
            {
                var obstructingPlayer = session.Players
                    .FirstOrDefault(player =>
                        player.Role == PlayerRole.Animatronic &&
                        door.CollisionArea.Contains(player.Position)
                    );

                if (obstructingPlayer is not null)
                {
                    throw new InvalidOperationException(
                        $"The door cannot close because player " +
                        $"'{obstructingPlayer.PlayerId}' is obstructing it."
                    );
                }
            }

            door.IsClosed = command.IsClosed;

            _gameSessionStore.Save(session);

            return new DoorStateChangedEvent
            {
                MatchId = matchId,
                DoorId = door.DoorId,
                IsClosed = door.IsClosed,
                ChangedByPlayerId = player.PlayerId
            };
        }
    }
}