using GameService.Models;

namespace GameService.Contracts;

public class AnimatronicEmpoweredEvent
{
    public string Type { get; set; } =
        "AnimatronicEmpowered";

    public string MatchId { get; set; } =
        string.Empty;

    public string PlayerId { get; set; } =
        string.Empty;

    public AnimatronicType AnimatronicType {
        get;
        set;
    }
}