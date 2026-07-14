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

            Rooms =
            [
                new MapArea
                {
                    Id = "office",
                    Name = "Oficina",
                    X = 19.98f,
                    Y = 2.00f,
                    Width = 4.46f,
                    Height = 3.39f
                },
                new MapArea
                {
                    Id = "pirate_cove",
                    Name = "Pirate Cove",
                    X = 2.06f,
                    Y = 25.67f,
                    Width = 7.79f,
                    Height = 6.39f
                },
                new MapArea
                {
                    Id = "main_stage",
                    Name = "Escenario principal",
                    X = 17.73f,
                    Y = 42.24f,
                    Width = 8.93f,
                    Height = 4.38f
                },
                new MapArea
                {
                    Id = "kitchen",
                    Name = "Cocina",
                    X = 31.15f,
                    Y = 13.14f,
                    Width = 8.99f,
                    Height = 7.79f
                },
                new MapArea
                {
                    Id = "parts_service",
                    Name = "Partes y servicio",
                    X = 2.91f,
                    Y = 36.57f,
                    Width = 4.77f,
                    Height = 5.60f
                },
                new MapArea
                {
                    Id = "supply_closet",
                    Name = "Supply closet",
                    X = 8.78f,
                    Y = 10.92f,
                    Width = 4.48f,
                    Height = 5.03f
                },
                new MapArea
                {
                    Id = "left_hall",
                    Name = "Pasillo izquierdo",
                    X = 14.38f,
                    Y = 2.00f,
                    Width = 4.51f,
                    Height = 20.09f
                },
                new MapArea
                {
                    Id = "right_hall",
                    Name = "Pasillo derecho",
                    X = 25.54f,
                    Y = 2.00f,
                    Width = 4.49f,
                    Height = 20.08f
                },
                new MapArea
                {
                    Id = "bathroom_hall",
                    Name = "Pasillo de baños",
                    X = 36.23f,
                    Y = 23.18f,
                    Width = 3.89f,
                    Height = 18.99f
                },
                new MapArea
                {
                    Id = "men_bathroom",
                    Name = "Baño de hombres",
                    X = 40.68f,
                    Y = 24.28f,
                    Width = 7.28f,
                    Height = 3.37f
                },
                new MapArea
                {
                    Id = "women_bathroom",
                    Name = "Baño de mujeres",
                    X = 40.68f,
                    Y = 34.30f,
                    Width = 7.27f,
                    Height = 3.36f
                },
                new MapArea
                {
                    Id = "backstage",
                    Name = "Backstage",
                    X = 14.44f,
                    Y = 51.04f,
                    Width = 14.51f,
                    Height = 3.42f
                },
                new MapArea
                {
                    Id = "backstage_office",
                    Name = "Oficina de backstage",
                    X = 8.80f,
                    Y = 49.95f,
                    Width = 5.59f,
                    Height = 4.49f
                },
                new MapArea
                {
                    Id = "dining_room",
                    Name = "Comedor central",
                    X = 8.79f,
                    Y = 23.23f,
                    Width = 26.83f,
                    Height = 18.95f
                }
            ],

            WalkableAreas =
            [
                new MapArea
                {
                    Id = "dining_walkable",
                    Name = "Comedor central",
                    X = 8.79f,
                    Y = 23.23f,
                    Width = 26.83f,
                    Height = 18.95f
                },
                new MapArea
                {
                    Id = "left_hall_walkable",
                    Name = "Pasillo izquierdo",
                    X = 14.38f,
                    Y = 2.00f,
                    Width = 4.51f,
                    Height = 20.09f
                },
                new MapArea
                {
                    Id = "right_hall_walkable",
                    Name = "Pasillo derecho",
                    X = 25.54f,
                    Y = 2.00f,
                    Width = 4.49f,
                    Height = 20.08f
                },
                new MapArea
                {
                    Id = "office_walkable",
                    Name = "Oficina",
                    X = 19.98f,
                    Y = 2.00f,
                    Width = 4.46f,
                    Height = 3.39f
                },
                new MapArea
                {
                    Id = "pirate_cove_walkable",
                    Name = "Pirate Cove",
                    X = 2.06f,
                    Y = 25.67f,
                    Width = 7.79f,
                    Height = 6.39f
                },
                new MapArea
                {
                    Id = "main_stage_walkable",
                    Name = "Escenario principal",
                    X = 17.73f,
                    Y = 42.24f,
                    Width = 8.93f,
                    Height = 4.38f
                },
                new MapArea
                {
                    Id = "kitchen_walkable",
                    Name = "Cocina",
                    X = 31.15f,
                    Y = 13.14f,
                    Width = 8.99f,
                    Height = 7.79f
                },
                new MapArea
                {
                    Id = "parts_service_walkable",
                    Name = "Partes y servicio",
                    X = 2.91f,
                    Y = 36.57f,
                    Width = 4.77f,
                    Height = 5.60f
                },
                new MapArea
                {
                    Id = "supply_closet_walkable",
                    Name = "Supply closet",
                    X = 8.78f,
                    Y = 10.92f,
                    Width = 4.48f,
                    Height = 5.03f
                },
                new MapArea
                {
                    Id = "bathroom_hall_walkable",
                    Name = "Pasillo de baños",
                    X = 36.23f,
                    Y = 23.18f,
                    Width = 3.89f,
                    Height = 18.99f
                },
                new MapArea
                {
                    Id = "men_bathroom_walkable",
                    Name = "Baño de hombres",
                    X = 40.68f,
                    Y = 24.28f,
                    Width = 7.28f,
                    Height = 3.37f
                },
                new MapArea
                {
                    Id = "women_bathroom_walkable",
                    Name = "Baño de mujeres",
                    X = 40.68f,
                    Y = 34.30f,
                    Width = 7.27f,
                    Height = 3.36f
                },
                new MapArea
                {
                    Id = "backstage_walkable",
                    Name = "Backstage",
                    X = 14.44f,
                    Y = 51.04f,
                    Width = 14.51f,
                    Height = 3.42f
                },
                new MapArea
                {
                    Id = "backstage_office_walkable",
                    Name = "Oficina de backstage",
                    X = 8.80f,
                    Y = 49.95f,
                    Width = 5.59f,
                    Height = 4.49f
                },
                // Conector comedor → pasillo izquierdo
                new MapArea
                {
                Id = "dining_left_hall_connector",
                Name = "Conector comedor - pasillo izquierdo",
                X = 15.35f,
                Y = 18.77f,
                Width = 2.54f,
                Height = 4.54f
                },

// Conector comedor → pasillo derecho
                new MapArea
                {
                    Id = "dining_right_hall_connector",
                    Name = "Conector comedor - pasillo derecho",
                    X = 26.56f,
                    Y = 18.81f,
                    Width = 2.50f,
                    Height = 4.44f
                },

// Conector comedor → cocina
                new MapArea
                {
                    Id = "dining_kitchen_connector",
                    Name = "Conector comedor - cocina",
                    X = 32.14f,
                    Y = 18.80f,
                    Width = 2.52f,
                    Height = 4.47f
                },

// Conector comedor → pasillo de baños
                new MapArea
                {
                    Id = "dining_bathroom_hall_connector",
                    Name = "Conector comedor - pasillo de baños",
                    X = 35.50f,
                    Y = 35.38f,
                    Width = 1.36f,
                    Height = 2.43f
                },

// Conector comedor → partes y servicio
                new MapArea
                {
                    Id = "dining_parts_service_connector",
                    Name = "Conector comedor - partes y servicio",
                    X = 7.48f,
                    Y = 39.79f,
                    Width = 1.84f,
                    Height = 1.39f
                },

// Conector escenario → backstage
                new MapArea
                {
                    Id = "stage_backstage_connector",
                    Name = "Conector escenario - backstage",
                    X = 20.88f,
                    Y = 46.51f,
                    Width = 2.60f,
                    Height = 4.61f
                },
                // Pasillo de baños → baño de hombres
                new MapArea
                {
                    Id = "bathroom_hall_men_connector",
                    Name = "Conector pasillo - baño de hombres",
                    X = 39.83f,
                    Y = 25.44f,
                    Width = 1.72f,
                    Height = 1.10f
                },

// Pasillo de baños → baño de mujeres
                new MapArea
                {
                    Id = "bathroom_hall_women_connector",
                    Name = "Conector pasillo - baño de mujeres",
                    X = 39.90f,
                    Y = 35.46f,
                    Width = 1.56f,
                    Height = 1.18f
                },

// Pasillo izquierdo → laundry / supply closet
                new MapArea
                {
                    Id = "left_hall_supply_connector",
                    Name = "Conector pasillo izquierdo - supply closet",
                    X = 12.95f,
                    Y = 13.14f,
                    Width = 1.60f,
                    Height = 1.13f
                },

// Pasillo izquierdo → oficina
                new MapArea
                {
                    Id = "left_hall_office_connector",
                    Name = "Conector pasillo izquierdo - oficina",
                    X = 18.57f,
                    Y = 4.21f,
                    Width = 1.62f,
                    Height = 1.14f
                },

// Pasillo derecho → oficina
                new MapArea
                {
                    Id = "right_hall_office_connector",
                    Name = "Conector pasillo derecho - oficina",
                    X = 24.15f,
                    Y = 4.21f,
                    Width = 1.65f,
                    Height = 1.14f
                }
            ],
            
            SecurityCameras =
            [
                new SecurityCameraDefinition
                {
                    CameraId = "cam_1a",
                    Name = "Escenario principal",
                    VisibleRoomIds =
                    [
                        "main_stage"
                    ]
                },
                new SecurityCameraDefinition
                {
                    CameraId = "cam_1b",
                    Name = "Comedor central",
                    VisibleRoomIds =
                    [
                        "dining_room"
                    ]
                },
                new SecurityCameraDefinition
                {
                    CameraId = "cam_1c",
                    Name = "Pirate Cove",
                    VisibleRoomIds =
                    [
                        "pirate_cove"
                    ]
                },
                new SecurityCameraDefinition
                {
                    CameraId = "cam_2a",
                    Name = "Pasillo izquierdo superior",
                    VisibleRoomIds =
                    [
                        "left_hall"
                    ]
                },
                new SecurityCameraDefinition
                {
                    CameraId = "cam_2b",
                    Name = "Pasillo izquierdo inferior",
                    VisibleRoomIds =
                    [
                        "left_hall"
                    ]
                },
                new SecurityCameraDefinition
                {
                    CameraId = "cam_3",
                    Name = "Supply closet",
                    VisibleRoomIds =
                    [
                        "supply_closet"
                    ]
                },
                new SecurityCameraDefinition
                {
                    CameraId = "cam_4a",
                    Name = "Pasillo derecho superior",
                    VisibleRoomIds =
                    [
                        "right_hall"
                    ]
                },
                new SecurityCameraDefinition
                {
                    CameraId = "cam_4b",
                    Name = "Pasillo derecho inferior",
                    VisibleRoomIds =
                    [
                        "right_hall"
                    ]
                },
                new SecurityCameraDefinition
                {
                    CameraId = "cam_5",
                    Name = "Partes y servicio",
                    VisibleRoomIds =
                    [
                        "parts_service"
                    ]
                },
                new SecurityCameraDefinition
                {
                    CameraId = "cam_6",
                    Name = "Cocina",
                    VisibleRoomIds =
                    [
                        "kitchen"
                    ]
                },
                new SecurityCameraDefinition
                {
                    CameraId = "cam_7",
                    Name = "Pasillo de baños",
                    VisibleRoomIds =
                    [
                        "bathroom_hall"
                    ]
                }
            ],

            BlockedAreas =
            [
                new MapArea
                {
                    Id = "dining_table_01",
                    Name = "Mesa del comedor 1",
                    X = 12.72f,
                    Y = 25.47f,
                    Width = 3.26f,
                    Height = 12.74f
                },
                new MapArea
                {
                    Id = "dining_table_02",
                    Name = "Mesa del comedor 2",
                    X = 18.29f,
                    Y = 25.43f,
                    Width = 3.33f,
                    Height = 12.81f
                },
                new MapArea
                {
                    Id = "dining_table_03",
                    Name = "Mesa del comedor 3",
                    X = 23.90f,
                    Y = 25.43f,
                    Width = 3.30f,
                    Height = 12.79f
                },
                new MapArea
                {
                    Id = "dining_table_04",
                    Name = "Mesa del comedor 4",
                    X = 29.49f,
                    Y = 25.45f,
                    Width = 3.29f,
                    Height = 12.79f
                },
                new MapArea
                {
                    Id = "parts_service_table",
                    Name = "Mesa de Partes y Servicio",
                    X = 3.20f,
                    Y = 36.57f,
                    Width = 2.19f,
                    Height = 4.46f
                },
                new MapArea
                {
                    Id = "backstage_table",
                    Name = "Mesa de backstage",
                    X = 25.55f,
                    Y = 52.19f,
                    Width = 2.24f,
                    Height = 1.11f
                },
                new MapArea
                {
                    Id = "backstage_office_table",
                    Name = "Mesa de oficina de backstage",
                    X = 8.75f,
                    Y = 49.95f,
                    Width = 1.13f,
                    Height = 3.35f
                },
                new MapArea
                {
                    Id = "backstage_office_cabinet",
                    Name = "Armario de oficina de backstage",
                    X = 11.01f,
                    Y = 53.66f,
                    Width = 3.31f,
                    Height = 0.80f
                },
                new MapArea
                {
                    Id = "dining_entrance_table",
                    Name = "Mesa de entrada al comedor",
                    X = 31.16f,
                    Y = 38.83f,
                    Width = 4.49f,
                    Height = 1.07f
                },
                new MapArea
                {
                    Id = "kitchen_table",
                    Name = "Mesa de cocina",
                    X = 32.26f,
                    Y = 13.11f,
                    Width = 6.71f,
                    Height = 3.40f
                },
                new MapArea
                {
                    Id = "kitchen_fridge_table",
                    Name = "Nevera y mesa de cocina",
                    X = 34.50f,
                    Y = 17.61f,
                    Width = 5.64f,
                    Height = 1.15f
                },
                new MapArea
                {
                    Id = "left_hall_tv",
                    Name = "Televisor del pasillo izquierdo",
                    X = 14.33f,
                    Y = 9.36f,
                    Width = 0.99f,
                    Height = 2.74f
                },
                new MapArea
                {
                    Id = "right_hall_trash",
                    Name = "Basura del pasillo derecho",
                    X = 28.91f,
                    Y = 3.79f,
                    Width = 1.13f,
                    Height = 3.70f
                },
                new MapArea
                {
                    Id = "right_hall_tv",
                    Name = "Televisor del pasillo derecho",
                    X = 29.11f,
                    Y = 14.96f,
                    Width = 0.93f,
                    Height = 1.49f
                },
                new MapArea
                {
                    Id = "right_hall_cabinet",
                    Name = "Armario del pasillo derecho",
                    X = 29.34f,
                    Y = 10.23f,
                    Width = 0.72f,
                    Height = 2.40f
                },
                new MapArea
                {
                    Id = "pirate_cove_boat",
                    Name = "Barco de Pirate Cove",
                    X = 3.08f,
                    Y = 31.35f,
                    Width = 4.53f,
                    Height = 0.77f
                },
                new MapArea
                {
                    Id = "pirate_cove_palm",
                    Name = "Palmera de Pirate Cove",
                    X = 3.55f,
                    Y = 26.05f,
                    Width = 3.37f,
                    Height = 0.71f
                },
                new MapArea
                {
                    Id = "dining_trash",
                    Name = "Basura del comedor",
                    X = 12.52f,
                    Y = 41.64f,
                    Width = 0.92f,
                    Height = 0.56f
                }
            ],

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