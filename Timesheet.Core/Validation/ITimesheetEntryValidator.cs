using System.ComponentModel.DataAnnotations;
using Timesheet.Core.Models.Dto;

namespace Timesheet.Core.Validation;

public interface ITimesheetEntryValidator
{
    ValidationResult? ValidateUnique(TimesheetEntryInsertDto dto, int? rowId = null);
}