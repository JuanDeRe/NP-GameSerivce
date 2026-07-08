using GameService.Contracts;
using GameService.Models;
using GameService.Services;

namespace GameService.Factories;

public class GameSessionFactory : IGameSessionFactory
{
    private readonly IMapService _mapService;

    public GameSessionFactory(IMapService mapService)
    {
        _mapService = mapService;
    }

    public GameSession Create(CreateMatchRequest request)
    {
        var matchId = $"match-{Guid.NewGuid():N}";

        var players = request.Players
            .Select((player, index) =>
            {
                var spawnPosition = GetSpawnPosition(player.Role, index);
                var roomId = _mapService.GetRoomId(spawnPosition);

                return new PlayerState
                {
                    PlayerId = player.PlayerId,
                    Role = player.Role,
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