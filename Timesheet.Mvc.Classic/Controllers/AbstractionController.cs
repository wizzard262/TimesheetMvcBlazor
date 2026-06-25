using Microsoft.AspNetCore.Mvc;

namespace Timesheet.Mvc.Classic.Controllers;

// Note: Controller only does orchestration, no business logic
public class AbstractionController() : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View("Index");
    }
}