using Timesheet.Core.Models.Dto;

namespace Timesheet.Mvc.Classic.ViewModels;

public class TimesheetRowViewModel
{
    public TimesheetEntryDto? TimesheetEntryDto { get; set; }
    public bool IsEditable { get; set; }
}
