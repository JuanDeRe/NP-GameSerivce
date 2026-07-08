using GameService.Models;

namespace GameService.Services;

public class MapService : IMapService
{
    private readonly GameMap _map;

    public MapService()
    {
        _map = BuildDemoMap();
    }

    public GameMap GetCurrentMap()
    {
        return _map;
    }

    public string GetRoomId(Position position)
    {
        return _map.GetRoomId(position);
    }

    public bool CanMoveTo(Position position)
    {
        return _map.IsInsideMap(position) && !_map.IsBlocked(position);
    }

    private static GameMap BuildDemoMap()
    {
        return new GameMap
        {
            MapId = "pizzeria-demo-map",
            MinX = 0,
            MinY = 0,
            MaxX = 100,
            MaxY = 60,
            Rooms =
            [
                new MapArea
                {
                    Id = "office",
                    Name = "Oficina",
                    X = 80,
                    Y = 20,
                    Width = 15,
                    Height = 15
                },
                new MapArea
                {
                    Id = "main_stage",
                    Name = "Escenario Principal",
                    X = 5,
                    Y = 40,
                    Width = 20,
                    Height = 12
                },
                new MapArea
                {
                    Id = "pirate_cove",
                    Name = "Pirate Cove",
                    X = 5,
                    Y = 10,
                    Width = 15,
                    Height = 12
                },
                new MapArea
                {
                    Id = "kitchen",
                    Name = "Cocina",
                    X = 35,
                    Y = 40,
                    Width = 15,
                    Height = 12
                },
                new MapArea
                {
                    Id = "parts_service",
                    Name = "Partes y Servicio",
                    X = 35,
                    Y = 8,
                    Width = 15,
                    Height = 12
                },
                new MapArea
                {
                    Id = "bathroom",
                    Name = "Baños",
                    X = 58,
                    Y = 8,
                    Width = 14,
                    Height = 12
                },
                new MapArea
                {
                    Id = "left_hall",
                    Name = "Pasillo Izquierdo",
                    X = 55,
                    Y = 35,
                    Width = 25,
                    Height = 10
                },
                new MapArea
                {
                    Id = "right_hall",
                    Name = "Pasillo Derecho",
                    X = 55,
                    Y = 15,
                    Width = 25,
                    Height = 10
                }
            ],
            BlockedAreas =
            [
                new MapArea
                {
                    Id = "wall_demo_01",
                    Name = "Pared Central",
                    X = 50,
                    Y = 25,
                    Width = 8,
                    Height = 10
                }
            ]
        };
    }
}