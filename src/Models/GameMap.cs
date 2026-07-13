namespace GameService.Models;

public class GameMap
{
    public string MapId { get; set; } = string.Empty;

    public float MinX { get; set; }

    public float MinY { get; set; }

    public float MaxX { get; set; }

    public float MaxY { get; set; }

    public List<MapArea> Rooms { get; set; } = [];

    public List<MapArea> BlockedAreas { get; set; } = [];
    
    
    public List<SpawnPoint> SpawnPoints { get; set; } = [];
    
    
    public List<GameplayZone> GameplayZones { get; set; } = [];
    
    public List<MapArea> WalkableAreas { get; set; } = [];

    public bool IsInsideMap(Position position)
    {
        return position.X >= MinX &&
               position.X <= MaxX &&
               position.Y >= MinY &&
               position.Y <= MaxY;
    }

    public bool IsBlocked(Position position)
    {
        return BlockedAreas.Any(area => area.Contains(position));
    }

    public string GetRoomId(Position position)
    {
        var room = Rooms.FirstOrDefault(area => area.Contains(position));
        return room?.Id ?? "unknown";
    }
    
    public bool IsWalkable(Position position)
    {
        return WalkableAreas.Any(
            area => area.Contains(position)
        );
    }
}