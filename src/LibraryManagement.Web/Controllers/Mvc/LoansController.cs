using LibraryManagement.Application.Commands.Books;
using LibraryManagement.Application.Commands.Loans;
using LibraryManagement.Application.Queries.Loans;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Web.Controllers.Mvc;

[Authorize]
public class LoansController : Controller
{
    private readonly IMediator _mediator;

    public LoansController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: Loans/MyLoans
    public async Task<IActionResult> MyLoans()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToAction("Login", "Account");
        }

        var query = new GetUserLoansQuery { UserId = userId };
        var loans = await _mediator.Send(query);

        return View(loans);
    }

    // POST: Loans/Return
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int loanId)
    {
        var command = new ReturnBookCommand { LoanId = loanId };
        var result = await _mediator.Send(command);

        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction(nameof(MyLoans));
    }
}