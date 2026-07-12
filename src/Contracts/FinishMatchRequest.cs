using GameService.Models;

namespace GameService.Contracts;

public class FinishMatchRequest
{
    public MatchWinner Winner { get; set; }
}