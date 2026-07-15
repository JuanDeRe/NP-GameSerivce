using GameService.Services;

namespace GameService.BackgroundServices;

/// <summary>
/// Periodically finishes matches whose timer has expired.
/// </summary>
public class MatchExpirationProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<MatchExpirationProcessor> _logger;

    public MatchExpirationProcessor(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<MatchExpirationProcessor> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("MatchExpirationProcessor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();

                var lifecycleService = scope.ServiceProvider
                    .GetRequiredService<IMatchLifecycleService>();

                var finishedMatches =
                    await lifecycleService
                        .FinishExpiredMatchesAsync();

                if (finishedMatches > 0)
                {
                    _logger.LogInformation(
                        "Expired matches finished: {FinishedMatches}",
                        finishedMatches
                    );
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error while finishing expired matches");
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}