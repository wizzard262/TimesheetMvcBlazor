using Timesheet.Core.Models.Dto;

namespace Timesheet.Core.Services;

public interface ITimesheetReportingService
{
    TimesheetReportDto GetReportModel(int? userId, string? isoWeekIdIsoYearId);
}