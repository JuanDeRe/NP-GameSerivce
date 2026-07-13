using GameService.Models;

public class DoorState
{
    public string DoorId { get; set; } = string.Empty;

    public bool IsClosed { get; set; }

    public MapArea CollisionArea { get; set; } = new();
}