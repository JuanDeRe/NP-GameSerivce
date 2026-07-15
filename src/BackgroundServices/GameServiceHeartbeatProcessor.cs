using GameService.Configuration;
using GameService.Models;
using GameService.Registry;
using GameService.Stores;
using Microsoft.Extensions.Options;

namespace GameService.BackgroundServices;

public class GameServiceHeartbeatProcessor
    : BackgroundService
{
    private readonly IGameServiceRegistry _registry;
    private readonly IGameSessionStore _sessionStore;
    private readonly GameServiceInstanceOptions _options;

    private readonly ILogger<GameServiceHeartbeatProcessor>
        _logger;

    public GameServiceHeartbeatProcessor(
        IGameServiceRegistry registry,
        IGameSessionStore sessionStore,
        IOptions<GameServiceInstanceOptions> options,
        ILogger<GameServiceHeartbeatProcessor> logger)
    {
        _registry = registry;
        _sessionStore = sessionStore;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "GameService instance {InstanceId} registering " +
            "with URL {InternalUrl}",
            _options.InstanceId,
            _options.InternalUrl
        );

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var activeMatches = _sessionStore
                    .GetAll()
                    .Count(match =>
                        match.Status == MatchStatus.Active
                    );

                var status =
                    activeMatches >= _options.MaxMatches
                        ? "Full"
                        : "Available";

                _registry.RegisterOrRefresh(
                    new GameServiceInstance
                    {
                        InstanceId =
                            _options.InstanceId,

                        InternalUrl =
                            _options.InternalUrl,

                        ActiveMatches =
                            activeMatches,

                        MaxMatches =
                            _options.MaxMatches,

                        Status = status
                    }
                );
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to refresh GameService heartbeat"
                );
            }

            try
            {
                var interval =
                    _options.HeartbeatIntervalSeconds <= 0
                        ? 5
                        : _options.HeartbeatIntervalSeconds;

                await Task.Delay(
                    TimeSpan.FromSeconds(interval),
                    stoppingToken
                );
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        try
        {
            _registry.RemoveInstance(
                _options.InstanceId
            );
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Failed to remove GameService instance " +
                "{InstanceId} during shutdown",
                _options.InstanceId
            );
        }
    }
}