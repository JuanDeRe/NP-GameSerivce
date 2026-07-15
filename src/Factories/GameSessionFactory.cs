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
    private const int RequiredItemsPerAnimatronic = 5;
    
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
                    AttackProgress = 0f,
                    CollectedItems = 0,
                    RequiredItems = player.Role == PlayerRole.Animatronic
                        ? 5
                        : 0,
                    IsEmpowered = false
                };
            })
            .ToList();
        var now = DateTime.UtcNow;
        var map = _mapService.GetCurrentMap();
        var collectibles = CreateCollectibles(
            players,
            map
        );

        return new GameSession
        {
            MatchId = matchId,
            Status = MatchStatus.Active,
            Winner = MatchWinner.None,
            DurationSeconds =
                _gameSettings.MatchDurationSeconds,

            Players = players,
            Collectibles = collectibles,

            Doors = CreateInitialDoors(),

            ActiveCameraId = null,

            SecurityCameras = map.SecurityCameras
                .Select(camera => new SecurityCameraState
                {
                    CameraId = camera.CameraId,
                    IsEnabled = camera.IsEnabled,
                    IsSabotaged = false
                })
                .ToList(),

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
    private static List<MatchCollectible> CreateCollectibles(
        IReadOnlyCollection<PlayerState> players,
        GameMap map)
    {
        var animatronics = players
            .Where(player =>
                player.Role == PlayerRole.Animatronic
            )
            .ToList();

        var requiredSpawnPoints =
            animatronics.Count * RequiredItemsPerAnimatronic;

        if (map.CollectibleSpawnPoints.Count < requiredSpawnPoints)
        {
            throw new InvalidOperationException(
                $"The map needs at least {requiredSpawnPoints} " +
                "collectible spawn points, but only " +
                $"{map.CollectibleSpawnPoints.Count} were configured."
            );
        }

        // Mezcla los puntos para que cambien entre partidas.
        var availableSpawnPoints = map.CollectibleSpawnPoints
            .OrderBy(_ => Random.Shared.Next())
            .ToList();

        var collectibles = new List<MatchCollectible>();
        var spawnIndex = 0;

        foreach (var player in animatronics)
        {
            var collectibleType = GetCollectibleType(
                player.AnimatronicType
            );

            for (var index = 0;
                 index < RequiredItemsPerAnimatronic;
                 index++)
            {
                var spawnPoint =
                    availableSpawnPoints[spawnIndex++];

                collectibles.Add(new MatchCollectible
                {
                    CollectibleId =
                        $"collectible-{Guid.NewGuid():N}",

                    Type = collectibleType,
                    OwnerPlayerId = player.PlayerId,

                    Position = new Position(
                        spawnPoint.Position.X,
                        spawnPoint.Position.Y
                    ),

                    RoomId = spawnPoint.RoomId,
                    IsCollected = false,
                    CollectedAtUtc = null
                });
            }
        }

        return collectibles;
    }
    private static CollectibleType GetCollectibleType(
        AnimatronicType animatronicType)
    {
        return animatronicType switch
        {
            AnimatronicType.Chica =>
                CollectibleType.ChicaItem,

            AnimatronicType.Bonnie =>
                CollectibleType.BonnieItem,

            AnimatronicType.Freddy =>
                CollectibleType.FreddyItem,

            AnimatronicType.Foxy =>
                CollectibleType.FoxyItem,

            _ => throw new InvalidOperationException(
                $"Animatronic type '{animatronicType}' " +
                "does not have a collectible type."
            )
        };
    }
}