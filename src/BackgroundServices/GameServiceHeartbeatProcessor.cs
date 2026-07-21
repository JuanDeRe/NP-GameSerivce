using GameService.Configuration;
using GameService.Models;
using GameService.Registry;
using GameService.Stores;
using Microsoft.Extensions.Options;
using Amazon.AutoScaling;
using Amazon.AutoScaling.Model;

namespace GameService.BackgroundServices;

public class GameServiceHeartbeatProcessor
    : BackgroundService
{
    private readonly IGameServiceRegistry _registry;
    private readonly IGameSessionStore _sessionStore;
    private readonly GameServiceInstanceOptions _options;
    private readonly IAmazonAutoScaling
        _autoScaling;

    private bool? _lastProtectionState;
    private readonly ILogger<GameServiceHeartbeatProcessor>
        _logger;

    public GameServiceHeartbeatProcessor(
        IGameServiceRegistry registry,
        IGameSessionStore sessionStore,
        IOptions<GameServiceInstanceOptions> options,
        IAmazonAutoScaling autoScaling,
        ILogger<GameServiceHeartbeatProcessor> logger)
    {
        _registry = registry;
        _sessionStore = sessionStore;
        _options = options.Value;
        _autoScaling = autoScaling;
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
        _logger.LogInformation(
            "Scale-in protection configuration. " +
            "InstanceId={InstanceId}, " +
            "AutoScalingGroupName={AutoScalingGroupName}",
            _options.InstanceId,
            _options.AutoScalingGroupName
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
                await UpdateScaleInProtectionAsync(
                    activeMatches > 0,
                    stoppingToken
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
    private async Task UpdateScaleInProtectionAsync(
        bool shouldBeProtected,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(
                _options.AutoScalingGroupName) ||
            string.IsNullOrWhiteSpace(
                _options.InstanceId))
        {
            _logger.LogWarning(
                "Scale-in protection was skipped. " +
                "AutoScalingGroupNameConfigured={AsgConfigured}, " +
                "InstanceId={InstanceId}",
                !string.IsNullOrWhiteSpace(
                    _options.AutoScalingGroupName
                ),
                _options.InstanceId
            );

            return;
        }

        if (_lastProtectionState ==
            shouldBeProtected)
        {
            return;
        }

        try
        {
            await _autoScaling
                .SetInstanceProtectionAsync(
                    new SetInstanceProtectionRequest
                    {
                        AutoScalingGroupName =
                            _options
                                .AutoScalingGroupName,

                        InstanceIds =
                        [
                            _options.InstanceId
                        ],

                        ProtectedFromScaleIn =
                            shouldBeProtected
                    },
                    cancellationToken
                );

            _lastProtectionState =
                shouldBeProtected;

            _logger.LogInformation(
                "Updated scale-in protection. " +
                "InstanceId={InstanceId}, " +
                "ActiveMatchesProtected={Protected}",
                _options.InstanceId,
                shouldBeProtected
            );
        }
        catch (OperationCanceledException)
            when (cancellationToken
                      .IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to update scale-in protection " +
                "for instance {InstanceId}.",
                _options.InstanceId
            );
        }
    }
}