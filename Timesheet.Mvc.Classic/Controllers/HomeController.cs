
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Timesheet.Core.Models.Dto;
using Timesheet.Core.Services;
using Timesheet.Mvc.Classic.ViewModels;

namespace Timesheet.Mvc.Classic.Controllers;

// Note: Controller only does orchestration, no business logic
public class HomeController(ITimesheetService service) : Controller
{
    private readonly ITimesheetService _service = service;

    [HttpGet]
    public IActionResult Index()
    {
        return RedirectWithInsertDto(null);
    }

    [HttpPost]
    public IActionResult Add([Bind(Prefix = "NewTimesheetEntry")] TimesheetEntryInsertDto timesheetEntryInsertDto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Message"] = "Add Timesheet entry failed.";
            TempData["MessageType"] = "alert-danger";
            return RedirectWithInsertDto(timesheetEntryInsertDto);
        }

        try
        {
            var timesheetEntryDto = _service.Add(timesheetEntryInsertDto);
            TempData["Message"] = $"Timesheet entry added with ID #{timesheetEntryDto?.Id}";
            TempData["MessageType"] = "alert-success";
            return RedirectToAction("Index");
        }
        catch (ValidationException ex)
        {
            TempData["Message"] = ex.Message;
            TempData["MessageType"] = "alert-danger";
            return RedirectWithInsertDto(timesheetEntryInsertDto);
        }
    }

    [HttpPost]
    public IActionResult RowAction(TimesheetEntryDto timesheetEntryDto, string action)
    {
        var id = timesheetEntryDto.Id;
        if (action == "update")
        {
            try
            {
                _service.Validate(timesheetEntryDto);
            }
            catch (ValidationException ex)
            {
                TempData["Message"] = $"Update failed for timesheet entry #{timesheetEntryDto.Id}. {ex.Message} Row has been reset.";
                TempData["MessageType"] = "alert-danger";
                return RedirectToAction("Index");
            }

            try
            {
                _service.Update(timesheetEntryDto);
                TempData["Message"] = $"Timesheet entry #{id} updated successfully.";
                TempData["MessageType"] = "alert-success";
            }
            catch (ValidationException ex)
            {
                TempData["Message"] = ex.Message;
                TempData["MessageType"] = "alert-danger";
                return RedirectWithInsertDto(null);
            }
        }
        else if (action == "delete")
        {
            _service.Delete(id);
            TempData["Message"] = $"Timesheet entry #{id} deleted successfully.";
            TempData["MessageType"] = "alert-success";
        }

        return RedirectToAction("Index");
    }

    #region Authorization Examples 

    [Authorize(Roles = "IsAdmin")]
    public IActionResult AdminOnlyPage()
    {
        return View();
    }

    [Authorize(Roles = "IsUser")]
    public IActionResult UserOnlyPage()
    {
        return View();
    }

    [Authorize]
    public IActionResult MyRoles()
    {
        var model = new MyRolesViewModel
        {
            IsAuthenticated = User.Identity?.IsAuthenticated ?? false,
            Claims = User.Claims
                .Select(c => new ClaimItem { Type = c.Type, Value = c.Value })
                .ToList()
        };
        return View(model);
    }

    public class MyRolesViewModel
    {
        public bool IsAuthenticated { get; set; }
        public List<ClaimItem> Claims { get; set; } = new();
    }

    public class ClaimItem
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    #endregion Authorization Examples 

    private ViewResult RedirectWithInsertDto(TimesheetEntryInsertDto? timesheetEntryInsertDto)
    {
        timesheetEntryInsertDto ??= new TimesheetEntryInsertDto
        {
            Date = DateTime.Today
        };

        var timesheetEntryList = _service.GetAll();
        var homeViewModel = new HomepageViewModel
        {
            TimesheetEntries = [.. timesheetEntryList],
            NewTimesheetEntry = timesheetEntryInsertDto
        };
        return View("Index", homeViewModel);
    }
}