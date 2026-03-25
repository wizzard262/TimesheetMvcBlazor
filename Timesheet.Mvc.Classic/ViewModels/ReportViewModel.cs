using Timesheet.Core.Models.Dto;

namespace Timesheet.Mvc.Classic.ViewModels;

public class ReportViewModel
{
    public string? SelectedIsoWeekIdIsoYearId { get; set; } // ste:todo: We get both values from single select input (<option value="week-year"), Better strongly typed object list with asp-items.
    public int? SelectedUserId { get; set; }
    public string? SelectedIsoWeekRange { get; set; }
    public List<int>? UserIds { get; set; }
    public List<IsoWeekDto>? IsoWeeks { get; set; }
    public List<TimesheetEntryDto>? TimesheetEntries { get; set; }
    public IEnumerable<KeyValuePair<int, decimal>>? ProjectHours { get; set; }
}