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
        return _map.IsInsideMap(position) &&
               _map.IsWalkable(position) &&
               !_map.IsBlocked(position);
    }

    private static GameMap BuildDemoMap()
    {
        return new GameMap
        {
            MapId = "pizzeria-demo-map",
            MinX = 0,
            MinY = 0,
            MaxX = 50,
            MaxY = 60,

            Rooms = [],

            WalkableAreas =
            [
                new MapArea
                {
                    Id = "temporary_full_map",
                    Name = "Área transitable temporal",
                    X = 0,
                    Y = 0,
                    Width = 50,
                    Height = 60
                }
            ],

            BlockedAreas = [],

            SpawnPoints =
            [
                new SpawnPoint
                {
                    Id = "guard_spawn",
                    Role = PlayerRole.Guard,
                    AnimatronicType = AnimatronicType.None,
                    Position = new Position(22.20f, 5.92f),
                    RoomId = "office"
                },
                new SpawnPoint
                {
                    Id = "foxy_spawn",
                    Role = PlayerRole.Animatronic,
                    AnimatronicType = AnimatronicType.Foxy,
                    Position = new Position(6.06f, 29.51f),
                    RoomId = "pirate_cove"
                },
                new SpawnPoint
                {
                    Id = "freddy_spawn",
                    Role = PlayerRole.Animatronic,
                    AnimatronicType = AnimatronicType.Freddy,
                    Position = new Position(22.21f, 45.37f),
                    RoomId = "main_stage"
                },
                new SpawnPoint
                {
                    Id = "bonnie_spawn",
                    Role = PlayerRole.Animatronic,
                    AnimatronicType = AnimatronicType.Bonnie,
                    Position = new Position(19.83f, 45.98f),
                    RoomId = "main_stage"
                },
                new SpawnPoint
                {
                    Id = "chica_spawn",
                    Role = PlayerRole.Animatronic,
                    AnimatronicType = AnimatronicType.Chica,
                    Position = new Position(24.66f, 46.25f),
                    RoomId = "main_stage"
                }
            ],

            GameplayZones =
            [
                // Puerta izquierda
                new GameplayZone
                {
                    Id = "left_door_visibility",
                    Name = "Zona visible de la puerta izquierda",
                    Type = MapZoneType.DoorVisibility,
                    Side = "Left",
                    X = 14.35f,
                    Y = 2.00f,
                    Width = 4.49f,
                    Height = 8.83f
                },
                new GameplayZone
                {
                    Id = "left_attack_zone",
                    Name = "Zona de ataque izquierda",
                    Type = MapZoneType.Attack,
                    Side = "Left",
                    RequiredDoorId = "left_door",
                    X = 16.03f,
                    Y = 3.69f,
                    Width = 2.80f,
                    Height = 1.68f
                },

                // Puerta derecha
                new GameplayZone
                {
                    Id = "right_door_visibility",
                    Name = "Zona visible de la puerta derecha",
                    Type = MapZoneType.DoorVisibility,
                    Side = "Right",
                    X = 25.54f,
                    Y = 2.00f,
                    Width = 4.51f,
                    Height = 8.85f
                },
                new GameplayZone
                {
                    Id = "right_attack_zone",
                    Name = "Zona de ataque derecha",
                    Type = MapZoneType.Attack,
                    Side = "Right",
                    RequiredDoorId = "right_door",
                    X = 25.54f,
                    Y = 3.71f,
                    Width = 2.80f,
                    Height = 1.64f
                }
            ]
        };
    }
}