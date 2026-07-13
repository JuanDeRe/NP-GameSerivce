using GameService.Configuration;
using GameService.Contracts;
using GameService.Models;
using GameService.Services;
using Microsoft.Extensions.Options;

namespace GameService.Factories;

/// <summary>
/// Creates game sessions with validated players, spawns and initial state.
/// </summary>
public class GameSessionFactory : IGameSessionFactory
{
    private readonly IMapService _mapService;
    private readonly GameSettingsOptions _gameSettings;

    public GameSessionFactory(
        IMapService mapService,
        IOptions<GameSettingsOptions> gameSettings)
    {
        _mapService = mapService;
        _gameSettings = gameSettings.Value;
    }

    public GameSession Create(CreateMatchRequest request)
    {
        ValidatePlayers(request);

        var matchId = $"match-{Guid.NewGuid():N}";

        var players = request.Players
            .Select(player =>
            {
                var spawnPosition = GetSpawnPosition(
                    player.Role,
                    player.AnimatronicType
                );

                var roomId = _mapService.GetRoomId(spawnPosition);

                return new PlayerState
                {
                    PlayerId = player.PlayerId,
                    Role = player.Role,
                    AnimatronicType = player.AnimatronicType,
                    Position = spawnPosition,
                    CurrentRoomId = roomId,
                    IsConnected = true,
                    IsAttacking = false,
                    ActiveAttackZoneId = null,
                    AttackProgress = 0f
                };
            })
            .ToList();

        var now = DateTime.UtcNow;

        return new GameSession
        {
            MatchId = matchId,
            Status = MatchStatus.Active,
            Winner = MatchWinner.None,
            DurationSeconds = _gameSettings.MatchDurationSeconds,
            Players = players,
            Doors = CreateInitialDoors(),
            CreatedAtUtc = now,
            StartedAtUtc = now
        };
    }

    private static void ValidatePlayers(CreateMatchRequest request)
    {
        if (request.Players.Count == 0)
        {
            throw new InvalidOperationException(
                "A match must have at least one player."
            );
        }

        var guards = request.Players
            .Where(player => player.Role == PlayerRole.Guard)
            .ToList();

        if (guards.Count != 1)
        {
            throw new InvalidOperationException(
                "A match must contain exactly one guard."
            );
        }

        if (guards[0].AnimatronicType != AnimatronicType.None)
        {
            throw new InvalidOperationException(
                "The guard cannot have an animatronic type."
            );
        }

        var animatronicPlayers = request.Players
            .Where(player => player.Role == PlayerRole.Animatronic)
            .ToList();

        if (animatronicPlayers.Any(player =>
                player.AnimatronicType == AnimatronicType.None))
        {
            throw new InvalidOperationException(
                "Every animatronic player must have an animatronic type."
            );
        }

        var duplicatedTypes = animatronicPlayers
            .GroupBy(player => player.AnimatronicType)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicatedTypes.Count > 0)
        {
            throw new InvalidOperationException(
                $"Duplicated animatronic types are not allowed: " +
                $"{string.Join(", ", duplicatedTypes)}"
            );
        }
    }

    private Position GetSpawnPosition(
        PlayerRole role,
        AnimatronicType animatronicType)
    {
        var map = _mapService.GetCurrentMap();

        var spawn = map.SpawnPoints.FirstOrDefault(point =>
            point.Role == role &&
            point.AnimatronicType == animatronicType
        );

        if (spawn is null)
        {
            throw new InvalidOperationException(
                $"No spawn configured for {role} - {animatronicType}."
            );
        }

        return new Position(
            spawn.Position.X,
            spawn.Position.Y
        );
    }

    private static List<DoorState> CreateInitialDoors()
    {
        return
        [
            new DoorState
            {
                DoorId = "left_door",
                IsClosed = false,
                CollisionArea = new MapArea
                {
                    Id = "left_door_collision",
                    Name = "Colisión puerta izquierda",
                    X = 18.87f,
                    Y = 3.69f,
                    Width = 1.08f,
                    Height = 1.66f
                }
            },
            new DoorState
            {
                DoorId = "right_door",
                IsClosed = false,
                CollisionArea = new MapArea
                {
                    Id = "right_door_collision",
                    Name = "Colisión puerta derecha",
                    X = 24.49f,
                    Y = 3.70f,
                    Width = 1.04f,
                    Height = 1.68f
                }
            }
        ];
    }
}