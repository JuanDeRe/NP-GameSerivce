using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using GameService.Configuration;
using GameService.Models;
using GameService.Stores;
using Microsoft.Extensions.Options;

namespace GameService.Observability;

public sealed class CloudWatchGameMetricsPublisher
    : BackgroundService
{
    private const string ServiceName =
        "game-service";

    private readonly IGameSessionStore _sessionStore;

    private readonly IAmazonCloudWatch _cloudWatch;

    private readonly GameServiceInstanceOptions
        _gameServiceOptions;

    private readonly GameMetricsOptions
        _metricsOptions;

    private readonly ILogger<
        CloudWatchGameMetricsPublisher
    > _logger;

    public CloudWatchGameMetricsPublisher(
        IGameSessionStore sessionStore,
        IAmazonCloudWatch cloudWatch,
        IOptions<GameServiceInstanceOptions>
            gameServiceOptions,
        IOptions<GameMetricsOptions>
            metricsOptions,
        ILogger<CloudWatchGameMetricsPublisher>
            logger)
    {
        _sessionStore = sessionStore;
        _cloudWatch = cloudWatch;

        _gameServiceOptions =
            gameServiceOptions.Value;

        _metricsOptions =
            metricsOptions.Value;

        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        if (!_metricsOptions.Enabled)
        {
            _logger.LogInformation(
                "CloudWatch custom metrics are disabled."
            );

            return;
        }

        var intervalSeconds =
            _metricsOptions.PublishIntervalSeconds <= 0
                ? 60
                : _metricsOptions
                    .PublishIntervalSeconds;

        _logger.LogInformation(
            "CloudWatch GameService metrics publisher " +
            "started. Namespace={Namespace}, " +
            "Environment={Environment}, " +
            "IntervalSeconds={IntervalSeconds}",
            _metricsOptions.Namespace,
            _metricsOptions.Environment,
            intervalSeconds
        );

        await PublishAsync(stoppingToken);

        using var timer = new PeriodicTimer(
            TimeSpan.FromSeconds(intervalSeconds)
        );

        try
        {
            while (await timer.WaitForNextTickAsync(
                       stoppingToken))
            {
                await PublishAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation(
                "CloudWatch GameService metrics " +
                "publisher stopped."
            );
        }
    }

    private async Task PublishAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var activeMatches = _sessionStore
                .GetAll()
                .Count(match =>
                    match.Status == MatchStatus.Active
                );

            var maxMatches = Math.Max(
                1,
                _gameServiceOptions.MaxMatches
            );

            var capacityUtilization =
                activeMatches * 100.0 / maxMatches;

            var request = new PutMetricDataRequest
            {
                Namespace =
                    _metricsOptions.Namespace,

                MetricData = new List<MetricDatum>
                {
                    new()
                    {
                        MetricName =
                            "ActiveMatches",

                        Value = activeMatches,

                        Unit =
                            StandardUnit.Count,

                        Timestamp =
                            DateTime.UtcNow,

                        Dimensions =
                            CreateDimensions()
                    },
                    new()
                    {
                        MetricName =
                            "MatchCapacityUtilization",

                        Value =
                            capacityUtilization,

                        Unit =
                            StandardUnit.Percent,

                        Timestamp =
                            DateTime.UtcNow,

                        Dimensions =
                            CreateDimensions()
                    }
                }
            };

            await _cloudWatch.PutMetricDataAsync(
                request,
                cancellationToken
            );

            _logger.LogInformation(
                "Published GameService metrics. " +
                "ActiveMatches={ActiveMatches}, " +
                "MaxMatches={MaxMatches}, " +
                "MatchCapacityUtilization=" +
                "{CapacityUtilization:F2}",
                activeMatches,
                maxMatches,
                capacityUtilization
            );
        }
        catch (OperationCanceledException)
            when (cancellationToken
                .IsCancellationRequested)
        {
            // Normal application shutdown.
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Failed to publish GameService " +
                "metrics to CloudWatch."
            );
        }
    }

    private List<Dimension> CreateDimensions()
    {
        return new List<Dimension>
        {
            new()
            {
                Name = "Environment",
                Value =
                    _metricsOptions.Environment
            },
            new()
            {
                Name = "Service",
                Value = ServiceName
            }
        };
    }
}