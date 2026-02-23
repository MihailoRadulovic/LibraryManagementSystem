using LibraryManagement.Application.Commands.Loans;
using LibraryManagement.Application.Queries.Loans;  
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Web.Controllers.Mvc;

[Authorize(Policy = "RequireLibrarianRole")]
public class LibrarianController : Controller
{
    private readonly IMediator _mediator;

    public LibrarianController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: Librarian/PendingLoans
    public async Task<IActionResult> PendingLoans()
    {
        var query = new GetPendingLoansQuery();
        var loans = await _mediator.Send(query);
        return View(loans);
    }

    // POST: Librarian/ApproveLoan
    [HttpPost]
    public async Task<IActionResult> ApproveLoan(int loanId, bool approve)
    {
        var librarianUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(librarianUserId))
        {
            TempData["ErrorMessage"] = "Nije moguće identifikovati bibliotekara.";
            return RedirectToAction(nameof(PendingLoans));
        }

        var command = new ApproveLoanCommand
        {
            LoanId = loanId,
            LibrarianUserId = librarianUserId,
            Approve = approve
        };

        var result = await _mediator.Send(command);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

        return RedirectToAction(nameof(PendingLoans));
    }

    // GET: Librarian/PendingReturns
    public async Task<IActionResult> PendingReturns()
    {
        var query = new GetPendingReturnsQuery();
        var returns = await _mediator.Send(query);
        return View(returns);
    }

    // POST: Librarian/ConfirmReturn
    [HttpPost]
    public async Task<IActionResult> ConfirmReturn(int loanId)
    {
        var librarianUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(librarianUserId))
        {
            TempData["ErrorMessage"] = "Nije moguće identifikovati bibliotekara.";
            return RedirectToAction(nameof(PendingReturns));
        }

        var command = new ConfirmReturnCommand
        {
            LoanId = loanId,
            LibrarianUserId = librarianUserId
        };

        var result = await _mediator.Send(command);

        if (result.Success)
        {
            var fineText = result.FineAmount > 0 ? $" Kazna: {result.FineAmount:C}" : "";
            TempData["SuccessMessage"] = result.Message + fineText;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction(nameof(PendingReturns));
    }
}