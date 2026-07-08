using Microsoft.AspNetCore.Mvc;
using GameService.Contracts;
using GameService.Services;

namespace GameService.Controllers;

[ApiController]
[Route("game/matches/{matchId}/gameplay")]
public class GameplayController : ControllerBase
{
    private readonly IMovementService _movementService;

    public GameplayController(IMovementService movementService)
    {
        _movementService = movementService;
    }

    [HttpPost("move")]
    public ActionResult<PlayerMovedEvent> Move(
        string matchId,
        [FromBody] MoveCommand command)
    {
        try
        {
            var movementEvent = _movementService.Move(matchId, command);
            return Ok(movementEvent);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }
}