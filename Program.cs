using System.Text.Json.Serialization;
using GameService.Factories;
using GameService.Stores;
using GameService.Services;
using GameService.BackgroundServices;
using GameService.Configuration;

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

builder.Services.AddScoped<IGameSessionService, GameSessionService>();
builder.Services.AddScoped<IMovementService, MovementService>();
builder.Services.AddScoped<IDoorService, DoorService>();

var app = builder.Build();

app.MapControllers();

app.Run();