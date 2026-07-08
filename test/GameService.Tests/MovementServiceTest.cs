using GameService.Contracts;
using GameService.Models;
using GameService.Services;
using GameService.Stores;
using Xunit;
namespace GameService.test.GameService.Tests;

public class MovementServiceTests
{
    [Fact]
    public void Move_Should_Update_Position_When_Target_Is_Valid()
    {
        var store = new InMemoryGameSessionStore();
        var mapService = new MapService();
        var movementService = new MovementService(store, mapService);

        var session = new GameSession
        {
            MatchId = "match-test",
            Status = MatchStatus.Active,
            Players =
            [
                new PlayerState
                {
                    PlayerId = "player-002",
                    Role = PlayerRole.Animatronic,
                    Position = new Position(10, 15),
                    CurrentRoomId = mapService.GetRoomId(new Position(10, 15))
                }
            ]
        };

        store.Save(session);

        var result = movementService.Move("match-test", new MoveCommand
        {
            PlayerId = "player-002",
            DirectionX = 1,
            DirectionY = 0,
            DeltaTime = 0.1f
        });

        Assert.Equal("PlayerMoved", result.Type);
        Assert.Equal("player-002", result.PlayerId);
        Assert.True(result.X > 10);
        Assert.Equal(15f, result.Y, 3);
    }

    [Fact]
    public void Move_Should_Block_Invalid_Axis_But_Allow_Valid_Axis()
    {
        var store = new InMemoryGameSessionStore();
        var mapService = new MapService();
        var movementService = new MovementService(store, mapService);

        var session = new GameSession
        {
            MatchId = "match-test",
            Status = MatchStatus.Active,
            Players =
            [
                new PlayerState
                {
                    PlayerId = "player-002",
                    Role = PlayerRole.Animatronic,
                    Position = new Position(3, 0),
                    CurrentRoomId = "unknown"
                }
            ]
        };

        store.Save(session);

        var result = movementService.Move("match-test", new MoveCommand
        {
            PlayerId = "player-002",
            DirectionX = -1,
            DirectionY = -1,
            DeltaTime = 0.1f
        });

        Assert.True(result.X < 3);
        Assert.Equal(0f, result.Y, 3);
    }

    [Fact]
    public void Move_Should_Not_Allow_Guard_To_Move_Freely()
    {
        var store = new InMemoryGameSessionStore();
        var mapService = new MapService();
        var movementService = new MovementService(store, mapService);

        var session = new GameSession
        {
            MatchId = "match-test",
            Status = MatchStatus.Active,
            Players =
            [
                new PlayerState
                {
                    PlayerId = "player-001",
                    Role = PlayerRole.Guard,
                    Position = new Position(87, 27),
                    CurrentRoomId = "office"
                }
            ]
        };

        store.Save(session);

        Assert.Throws<InvalidOperationException>(() =>
            movementService.Move("match-test", new MoveCommand
            {
                PlayerId = "player-001",
                DirectionX = 1,
                DirectionY = 0,
                DeltaTime = 0.1f
            })
        );
    }

    [Fact]
    public void Move_Should_Throw_When_Match_Does_Not_Exist()
    {
        var store = new InMemoryGameSessionStore();
        var mapService = new MapService();
        var movementService = new MovementService(store, mapService);

        Assert.Throws<InvalidOperationException>(() =>
            movementService.Move("invalid-match", new MoveCommand
            {
                PlayerId = "player-002",
                DirectionX = 1,
                DirectionY = 0,
                DeltaTime = 0.1f
            })
        );
    }
}