using GameService.Models;
namespace GameService.Contracts;


public class CreateMatchPlayerDto
{
    public string PlayerId { get; set; } = string.Empty;

    public PlayerRole Role { get; set; }
}