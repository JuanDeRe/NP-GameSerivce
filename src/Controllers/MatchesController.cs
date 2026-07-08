using Microsoft.AspNetCore.Mvc;
using GameService.Contracts;
using GameService.Services;

namespace GameService.Controllers;

[ApiController]
[Route("game/matches")]
public class MatchesController : ControllerBase
{
    private readonly IGameSessionService _gameSessionService;

    public MatchesController(IGameSessionService gameSessionService)
    {
        _gameSessionService = gameSessionService;
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
}