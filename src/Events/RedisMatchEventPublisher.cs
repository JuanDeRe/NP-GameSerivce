using System.Text.Json;
using GameService.Configuration;
using GameService.Contracts;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace GameService.Events;

/// <summary>
/// Persists match lifecycle events in a Redis Stream.
/// </summary>
public class RedisMatchEventPublisher
    : IMatchEventPublisher
{
    private const string MatchEventsStream =
        "nightwatch:streams:match-events";

    private readonly IConnectionMultiplexer _redis;
    private readonly RedisOptions _options;

    public RedisMatchEventPublisher(
        IConnectionMultiplexer redis,
        IOptions<RedisOptions> options)
    {
        _redis = redis;
        _options = options.Value;
    }

    public Task PublishMatchFinishedAsync(
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

        // Primero persistimos el estado final.
        database.StringSet(
            stateKey,
            json,
            TimeSpan.FromHours(24)
        );

        // Después agregamos el evento durable al Stream.
        database.StreamAdd(
            MatchEventsStream,
            [
                new NameValueEntry(
                    "eventType",
                    "MatchFinished"
                ),
                new NameValueEntry(
                    "eventId",
                    matchFinishedEvent.EventId
                ),
                new NameValueEntry(
                    "matchId",
                    matchFinishedEvent.MatchId
                ),
                new NameValueEntry(
                    "payload",
                    json
                )
            ],
            maxLength: 10_000,
            useApproximateMaxLength: true
        );

        return Task.CompletedTask;
    }
}