using System.Text.Json;
using GameService.Configuration;
using GameService.Contracts;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace GameService.Events;

/// <summary>
/// Persists and publishes match lifecycle events through Redis.
/// </summary>
public class RedisMatchEventPublisher
    : IMatchEventPublisher
{
    private const string MatchFinishedChannel =
        "nightwatch:events:match-finished";

    private readonly IConnectionMultiplexer _redis;
    private readonly RedisOptions _options;

    public RedisMatchEventPublisher(
        IConnectionMultiplexer redis,
        IOptions<RedisOptions> options)
    {
        _redis = redis;
        _options = options.Value;
    }

    public async Task PublishMatchFinishedAsync(
        MatchFinishedEvent matchFinishedEvent)
    {
        var json = JsonSerializer.Serialize(
            matchFinishedEvent
        );

        var database = _redis.GetDatabase();

        var stateKey =
            $"{_options.InstanceName}" +
            $"match-finished:" +
            $"{matchFinishedEvent.MatchId}";

        // Persistimos primero el resultado para poder reconciliarlo.
        await database.StringSetAsync(
            stateKey,
            json,
            TimeSpan.FromHours(24)
        );

        var subscriber = _redis.GetSubscriber();

        await subscriber.PublishAsync(
            RedisChannel.Literal(
                MatchFinishedChannel
            ),
            json
        );
    }
}