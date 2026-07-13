namespace GameService.Models;

public class GameplayZone : MapArea
{
    public MapZoneType Type { get; set; }

    public string? Side { get; set; }

    public string? RequiredDoorId { get; set; }
}