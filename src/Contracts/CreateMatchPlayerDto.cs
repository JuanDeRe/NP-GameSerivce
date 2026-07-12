using GameService.Models;

namespace GameService.Contracts;

public class CreateMatchPlayerDto
{
    public string PlayerId { get; set; } = string.Empty;

    public PlayerRole Role { get; set; }

    public AnimatronicType AnimatronicType { get; set; }
        = AnimatronicType.None;
}