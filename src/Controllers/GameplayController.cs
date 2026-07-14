using Microsoft.AspNetCore.Mvc;
using GameService.Contracts;
using GameService.Services;

namespace GameService.Controllers;

[ApiController]
[Route("game/matches/{matchId}/gameplay")]
public class GameplayController : ControllerBase
{
    private readonly IMovementService _movementService;
    private readonly IDoorService _doorService;
    private readonly ICameraService _cameraService;

    public GameplayController(
        IMovementService movementService,
        IDoorService doorService,
        ICameraService cameraService)
    {
        _movementService = movementService;
        _doorService = doorService;
        _cameraService = cameraService;
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
    [HttpPost("doors/state")]
    public ActionResult<DoorStateChangedEvent> SetDoorState(
        string matchId,
        [FromBody] SetDoorStateCommand command)
    {
        try
        {
            var doorEvent = _doorService.SetDoorState(
                matchId,
                command
            );

            return Ok(doorEvent);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }
    [HttpPost("cameras/active")]
    public ActionResult<ActiveCameraChangedEvent>
        ChangeCamera(
            string matchId,
            [FromBody] ChangeCameraCommand command)
    {
        try
        {
            var cameraEvent =
                _cameraService.ChangeCamera(
                    matchId,
                    command
                );

            return Ok(cameraEvent);
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