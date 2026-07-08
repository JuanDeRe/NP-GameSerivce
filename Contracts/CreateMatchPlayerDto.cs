namespace GameService.Contracts;

using GameService.Models;

public class CreateMatchPlayerDto
{
    public string PlayerId { get; set; } = string.Empty;

    public PlayerRole Role { get; set; }
}