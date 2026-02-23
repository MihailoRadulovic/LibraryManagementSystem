using LibraryManagement.Application.Commands.Reservations;
using LibraryManagement.Application.Queries.Reservations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Web.Controllers.Mvc;

[Authorize]
public class ReservationsController : Controller
{
    private readonly IMediator _mediator;

    public ReservationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var query = new GetUserReservationsQuery { UserId = userId };
        var reservations = await _mediator.Send(query);
        
        return View(reservations);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int bookId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var command = new CreateReservationCommand
        {
            BookId = bookId,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction("Details", "Books", new { id = bookId });
    }

    // POST: Reservations/Pickup
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pickup(int reservationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var command = new PickupReservationCommand
        {
            ReservationId = reservationId,
            UserId = userId
        };

        var result = await _mediator.Send(command);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("MyLoans", "Loans");
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        try
        {
            var command = new CancelReservationCommand
            {
                ReservationId = id,
                UserId = userId
            };

            await _mediator.Send(command);
            TempData["SuccessMessage"] = "Rezervacija je otkazana.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Queue(int bookId)
    {
        var query = new GetReservationQueueQuery { BookId = bookId };
        var queue = await _mediator.Send(query);
        
        return View(queue);
    }
}
