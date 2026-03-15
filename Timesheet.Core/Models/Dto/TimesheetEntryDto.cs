using System.ComponentModel.DataAnnotations;

namespace Timesheet.Core.Models.Dto;

public class TimesheetEntryDto : TimesheetEntryInsertDto
{
    [Required]
    public int Id { get; set; }
}