using Microsoft.AspNetCore.Mvc;

namespace Timesheet.Mvc.RazorComponents.Controllers;

public class HomeController() : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View("Index", null);
    }
}