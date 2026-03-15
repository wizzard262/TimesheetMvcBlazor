using Microsoft.AspNetCore.Mvc;
using Timesheet.Core.Models.Dto;
using Timesheet.Core.Services;
using Timesheet.Mvc.Classic.ViewModels;

namespace Timesheet.Mvc.Classic.Controllers;

public class ReportingController(ITimesheetReportingService service) : Controller
{
    private readonly ITimesheetReportingService _service = service;

    [HttpGet]
    public IActionResult Index()
    {
        var timesheetReportDto = _service.GetReportModel(null, null);
        var reportViewModel = MapTimesheetReportDtoToReportViewModel(timesheetReportDto);
        return View("Index", reportViewModel);
    }

    [HttpPost]
    public IActionResult SubmitReport(int selectedUserId, string selectedIsoWeekIdIsoYearId)
    {
        var timesheetReportDto = _service.GetReportModel(selectedUserId, selectedIsoWeekIdIsoYearId);
        var reportViewModel = MapTimesheetReportDtoToReportViewModel(timesheetReportDto);
        return View("Index", reportViewModel);
    }

    private static ReportViewModel MapTimesheetReportDtoToReportViewModel(TimesheetReportDto timesheetReportDto)
    {
        return new ReportViewModel
        {
            SelectedUserId = timesheetReportDto.SelectedUserId,
            SelectedIsoWeekIdIsoYearId = timesheetReportDto.SelectedIsoWeekIdIsoYearId,
            SelectedIsoWeekRange = timesheetReportDto.SelectedIsoWeekRange,
            UserIds = timesheetReportDto.UserIds,
            IsoWeeks = timesheetReportDto.IsoWeeks,
            TimesheetEntries = timesheetReportDto.TimesheetEntries,
            ProjectHours = timesheetReportDto.ProjectHours
        };
    }
}