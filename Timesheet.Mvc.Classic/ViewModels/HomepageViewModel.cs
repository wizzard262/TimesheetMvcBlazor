using Timesheet.Core.Models.Dto;

namespace Timesheet.Mvc.Classic.ViewModels;

public class HomepageViewModel
{
    public List<TimesheetEntryDto>? TimesheetEntries { get; set; }
    public TimesheetEntryInsertDto? NewTimesheetEntry { get; set; }
}