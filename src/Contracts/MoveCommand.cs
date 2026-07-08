namespace GameService.Contracts;

public class MoveCommand

{
    public string PlayerId { get; set; } = string.Empty;

    public float DirectionX { get; set; }

    public float DirectionY { get; set; }

    public float DeltaTime { get; set; } = 0.016f;
}