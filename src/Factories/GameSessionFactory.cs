using GameService.Contracts;
using GameService.Models;
using GameService.Services;
using GameService.Configuration;
using Microsoft.Extensions.Options;

namespace GameService.Factories;

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
        var matchId = $"match-{Guid.NewGuid():N}";
        
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
                $"Duplicated animatronic types are not allowed: {string.Join(", ", duplicatedTypes)}"
            );
        }

        var players = request.Players
            .Select((player, index) =>
            {
                var spawnPosition = GetSpawnPosition(player.Role, index);
                var roomId = _mapService.GetRoomId(spawnPosition);

                return new PlayerState
                {
                    PlayerId = player.PlayerId,
                    Role = player.Role,
                    AnimatronicType = player.AnimatronicType,
                    Position = spawnPosition,
                    CurrentRoomId = roomId,
                    IsConnected = true
                };
            })
            .ToList();

        return new GameSession
        {
            MatchId = matchId,
            Status = MatchStatus.Active,
            Winner = MatchWinner.None,
            DurationSeconds = _gameSettings.MatchDurationSeconds,
            Players = players,
            CreatedAtUtc = DateTime.UtcNow,
            StartedAtUtc = DateTime.UtcNow
        };
    }

    private static Position GetSpawnPosition(PlayerRole role, int index)
    {
        if (role == PlayerRole.Guard)
        {
            return new Position(87, 27);
        }

        var animatronicSpawns = new List<Position>
        {
            new(10, 45),
            new(10, 15),
            new(40, 45),
            new(40, 12),
            new(62, 12)
        };

        return animatronicSpawns[index % animatronicSpawns.Count];
    }
}