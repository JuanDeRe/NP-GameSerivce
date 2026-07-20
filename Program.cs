using System.Text.Json.Serialization;
using GameService.Factories;
using GameService.Stores;
using GameService.Services;
using GameService.BackgroundServices;
using GameService.Configuration;
using GameService.Events;
using StackExchange.Redis;
using GameService.Registry;
using Amazon.CloudWatch;
using GameService.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<GameSettingsOptions>(
    builder.Configuration.GetSection("GameSettings")
);

builder.Services.AddScoped<IMatchLifecycleService, MatchLifecycleService>();
builder.Services.AddSingleton<IMapService, MapService>();
builder.Services.AddSingleton<IGameSessionStore, InMemoryGameSessionStore>();
builder.Services.AddSingleton<IGameSessionFactory, GameSessionFactory>();
builder.Services.AddHostedService<MatchExpirationProcessor>();
builder.Services.AddScoped<ICameraService, CameraService>();
builder.Services.AddScoped<IGameSessionService, GameSessionService>();
builder.Services.AddScoped<IMovementService, MovementService>();
builder.Services.AddScoped<IDoorService, DoorService>();
builder.Services.AddSingleton<IMatchEventPublisher,RedisMatchEventPublisher>();
builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("Redis"));
builder.Services.AddSingleton<IConnectionMultiplexer>(
    serviceProvider =>
    {
        var redisOptions = serviceProvider
            .GetRequiredService<
                Microsoft.Extensions.Options.IOptions<
                    RedisOptions
                >
            >()
            .Value;

        var configuration =
            ConfigurationOptions.Parse(
                redisOptions.ConnectionString
            );

        configuration.AbortOnConnectFail = false;
        configuration.ConnectRetry = 3;
        configuration.ConnectTimeout = 5000;

        return ConnectionMultiplexer.Connect(
            configuration
        );
    }
);
builder.Services.Configure<
    GameServiceInstanceOptions
>(
    builder.Configuration.GetSection(
        "GameService"
    )
);

builder.Services.AddSingleton<
    IGameServiceRegistry,
    RedisGameServiceRegistry
>();

builder.Services.AddHostedService<
    GameServiceHeartbeatProcessor
>();
builder.Services.Configure<GameMetricsOptions>(
    builder.Configuration.GetSection(
        GameMetricsOptions.SectionName
    )
);
builder.Services.AddSingleton<
    IAmazonCloudWatch
>(
    _ => new AmazonCloudWatchClient()
);
builder.Services.AddHostedService<
    CloudWatchGameMetricsPublisher
>();
builder.Services.AddHealthChecks();
var app = builder.Build();

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();