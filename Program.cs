using System.Text.Json.Serialization;
using GameService.Factories;
using GameService.Stores;
using GameService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddSingleton<IMapService, MapService>();
builder.Services.AddSingleton<IGameSessionStore, InMemoryGameSessionStore>();
builder.Services.AddSingleton<IGameSessionFactory, GameSessionFactory>();

builder.Services.AddScoped<IGameSessionService, GameSessionService>();
builder.Services.AddScoped<IMovementService, MovementService>();

var app = builder.Build();

app.MapControllers();

app.Run();