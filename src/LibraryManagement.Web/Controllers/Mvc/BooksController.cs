using LibraryManagement.Application.Commands.Books;
using LibraryManagement.Application.Commands.Reservations;
using LibraryManagement.Application.Queries.Books;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;



namespace LibraryManagement.Web.Controllers.Mvc;

public class BooksController : Controller
{
    private readonly IMediator _mediator;

    public BooksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? genre, bool? onlyAvailable)
    {
        var query = new GetAllBooksQuery
        {
            SearchTerm = searchTerm,
            Genre = genre,
            OnlyAvailable = onlyAvailable
        };
        var books = await _mediator.Send(query);

        ViewBag.SearchTerm = searchTerm;
        ViewBag.Genre = genre;
        ViewBag.OnlyAvailable = onlyAvailable;

        return View(books);
    }

    public async Task<IActionResult> Details(int id)
    {
        var query = new GetBookByIdQuery { Id = id };
        var book = await _mediator.Send(query);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [Authorize(Policy = "RequireLibrarianRole")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(int bookId)
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

        return RedirectToAction(nameof(Details), new { id = bookId });
    }

    [HttpPost]
    [Authorize(Policy = "RequireLibrarianRole")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBookCommand command)
    {
        if (!ModelState.IsValid)
        {
            return View(command);
        }

        try
        {
            var bookId = await _mediator.Send(command);
            TempData["SuccessMessage"] = "Knjiga je uspešno dodata.";
            return RedirectToAction(nameof(Details), new { id = bookId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(command);
        }
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(int bookId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var command = new BorrowBookCommand
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

        return RedirectToAction(nameof(Details), new { id = bookId });
    }
}