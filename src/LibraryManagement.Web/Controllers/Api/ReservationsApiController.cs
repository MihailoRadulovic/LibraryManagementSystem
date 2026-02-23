using LibraryManagement.Application.Commands.Reservations;
using LibraryManagement.Application.Queries.Reservations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsApiController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReservationsApiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Kreira rezervaciju knjige - SLOŽEN slučaj korišćenja sa redom čekanja
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Otkazuje rezervaciju
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelReservationCommand command)
    {
        try
        {
            command.ReservationId = id;
            var success = await _mediator.Send(command);
            return Ok(new { success, message = "Rezervacija je otkazana." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Vraća red čekanja za knjugu
    /// </summary>
    [HttpGet("queue/{bookId}")]
    public async Task<IActionResult> GetQueue(int bookId)
    {
        var query = new GetReservationQueueQuery { BookId = bookId };
        var queue = await _mediator.Send(query);
        return Ok(queue);
    }

    /// <summary>
    /// Vraća sve rezervacije korisnika
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserReservations(string userId)
    {
        var query = new GetUserReservationsQuery { UserId = userId };
        var reservations = await _mediator.Send(query);
        return Ok(reservations);
    }
}
