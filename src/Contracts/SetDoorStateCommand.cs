namespace GameService.Contracts;

public class SetDoorStateCommand
{
    public string PlayerId { get; set; } = string.Empty;

    public string DoorId { get; set; } = string.Empty;

    public bool IsClosed { get; set; }
}