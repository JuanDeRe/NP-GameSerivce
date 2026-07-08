namespace GameService.Models;

public class MapArea
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public float X { get; set; }

    public float Y { get; set; }

    public float Width { get; set; }

    public float Height { get; set; }

    public bool Contains(Position position)
    {
        return position.X >= X &&
               position.X <= X + Width &&
               position.Y >= Y &&
               position.Y <= Y + Height;
    }
}