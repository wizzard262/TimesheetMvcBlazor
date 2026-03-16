using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Services;

namespace Timesheet.Mvc.RazorComponents.Controllers;

public class ReportingController(ITimesheetReportingService service) : Controller
{
    private readonly ITimesheetReportingService _service = service;

    [HttpGet]
    public IActionResult Index()
    {
        return View("Index", null);
    }

    [HttpPost]
    public IActionResult SubmitReport(int selectedUserId, string selectedIsoWeekIdIsoYearId)
    {
        return View("Index", null);
    }


}