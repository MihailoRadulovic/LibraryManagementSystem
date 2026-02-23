using LibraryManagement.Application.Commands.Books;
using LibraryManagement.Application.Queries.Books;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Application.Queries.Loans; 


namespace LibraryManagement.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BooksApiController : ControllerBase
{
    private readonly IMediator _mediator;

    public BooksApiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Vraća sve knjige sa opcijama filtriranja
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] string? searchTerm, [FromQuery] string? genre, [FromQuery] bool? onlyAvailable)
    {
        var query = new GetAllBooksQuery
        {
            SearchTerm = searchTerm,
            Genre = genre,
            OnlyAvailable = onlyAvailable
        };

        var books = await _mediator.Send(query);
        return Ok(books);
    }

    /// <summary>
    /// Vraća detalje jedne knjige
    /// </summary>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetBookByIdQuery { Id = id };
        var book = await _mediator.Send(query);

        if (book == null)
        {
            return NotFound(new { message = "Knjiga nije pronađena." });
        }

        return Ok(book);
    }

    /// <summary>
    /// Kreira novu knjigu (samo admin i bibliotekar)
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireLibrarianRole")]
    public async Task<IActionResult> Create([FromBody] CreateBookCommand command)
    {
        try
        {
            var bookId = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = bookId }, new { id = bookId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Pozajmljuje knjigu
    /// </summary>
    [HttpPost("borrow")]
    public async Task<IActionResult> Borrow([FromBody] BorrowBookCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Vraća pozajmljenu knjigu
    /// </summary>
    [HttpPost("return")]
    public async Task<IActionResult> Return([FromBody] ReturnBookCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Vraća sve pozajmice korisnika
    /// </summary>
    [HttpGet("loans/{userId}")]
    public async Task<IActionResult> GetUserLoans(string userId)
    {
        var query = new GetUserLoansQuery
        {
            UserId = userId
        };

        var result = await _mediator.Send(query);

        // Vraćamo sve kategorije
        return Ok(new
        {
            pending = result.PendingRequests,
            approved = result.ApprovedLoans,
            rejected = result.RejectedRequests,
            returned = result.ReturnedLoans
        });
    }
}
