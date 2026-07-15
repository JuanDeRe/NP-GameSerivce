using Microsoft.AspNetCore.Mvc;
using GameService.Contracts;
using GameService.Services;

namespace GameService.Controllers;

[ApiController]
[Route("game/matches")]
public class MatchesController : ControllerBase
{
    private readonly IGameSessionService _gameSessionService;
    private readonly IMatchLifecycleService _matchLifecycleService;

    public MatchesController(
        IGameSessionService gameSessionService,
        IMatchLifecycleService matchLifecycleService)
    {
        _gameSessionService = gameSessionService;
        _matchLifecycleService = matchLifecycleService;
    }

    [HttpPost]
    public ActionResult<CreateMatchResponse> CreateMatch([FromBody] CreateMatchRequest request)
    {
        if (request.Players.Count == 0)
        {
            return BadRequest(new
            {
                message = "A match must have at least one player."
            });
        }

        var response = _gameSessionService.CreateMatch(request);
        return Ok(response);
    }

    [HttpGet("{matchId}")]
    public IActionResult GetMatch(string matchId)
    {
        var match = _gameSessionService.GetMatch(matchId);

        if (match is null)
        {
            return NotFound(new
            {
                message = $"Match '{matchId}' was not found."
            });
        }

        return Ok(match);
    }

    [HttpGet]
    public IActionResult GetAllMatches()
    {
        var matches = _gameSessionService.GetAllMatches();
        return Ok(matches);
    }
    [HttpPost("{matchId}/finish")]
    public async Task<IActionResult> FinishMatch(
        string matchId,
        [FromBody] FinishMatchRequest request)
    {
        var match = await _matchLifecycleService
            .FinishMatchAsync(
                matchId,
                request.Winner
            );

        if (match is null)
        {
            return NotFound(new
            {
                message =
                    $"Match '{matchId}' was not found."
            });
        }

        return Ok(match);
    }
}