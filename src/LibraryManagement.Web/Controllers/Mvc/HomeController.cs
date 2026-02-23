using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.Controllers.Mvc;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View();
    }
}
