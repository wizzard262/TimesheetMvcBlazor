using Microsoft.AspNetCore.Mvc;

namespace Timesheet.Mvc.RazorComponents.Controllers;

public class AbstractionController() : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View("Index", null);
    }
}