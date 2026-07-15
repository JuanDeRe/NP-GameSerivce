using System.Text.Json;
using System.Text.Json.Serialization;
using GameService.Configuration;
using GameService.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace GameService.Registry;

public class RedisGameServiceRegistry
    : IGameServiceRegistry
{
    private static readonly JsonSerializerOptions JsonOptions =
        new()
        {
            PropertyNamingPolicy =
                JsonNamingPolicy.CamelCase,

            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

    private readonly IConnectionMultiplexer _redis;
    private readonly RedisOptions _options;

    public RedisGameServiceRegistry(
        IConnectionMultiplexer redis,
        IOptions<RedisOptions> options)
    {
        _redis = redis;
        _options = options.Value;
    }

    public void RegisterOrRefresh(
        GameServiceInstance instance)
    {
        var database = _redis.GetDatabase();

        instance.LastHeartbeatUtc = DateTime.UtcNow;

        var json = JsonSerializer.Serialize(
            instance,
            JsonOptions
        );

        database.StringSet(
            InstanceKey(instance.InstanceId),
            json,
            TimeSpan.FromSeconds(20)
        );

        database.SetAdd(
            InstancesSetKey,
            instance.InstanceId
        );
    }

    public void RemoveInstance(string instanceId)
    {
        var database = _redis.GetDatabase();

        database.KeyDelete(
            InstanceKey(instanceId)
        );

        database.SetRemove(
            InstancesSetKey,
            instanceId
        );
    }

    private string InstancesSetKey =>
        $"{_options.InstanceName}game-services:instances";

    private string InstanceKey(string instanceId) =>
        $"{_options.InstanceName}" +
        $"game-services:instance:{instanceId}";
}