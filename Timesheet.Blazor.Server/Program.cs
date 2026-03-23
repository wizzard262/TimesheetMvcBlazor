using Timesheet.Blazor.Server.Components;
using Timesheet.Core.Repositories;
using Timesheet.Core.Services;
using Timesheet.Core.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddSingleton<ITimesheetRepository, TimesheetRepository>();
builder.Services.AddScoped<ITimesheetService, TimesheetService>();
builder.Services.AddScoped<ITimesheetReportingService, TimesheetReportingService>();
builder.Services.AddScoped<ITimesheetEntryValidator, TimesheetEntryValidator>();

var webApplication = builder.Build();

// Configure the HTTP request pipeline.
if (!webApplication.Environment.IsDevelopment())
{
    webApplication.UseExceptionHandler("/Error", createScopeForErrors: true);
    webApplication.UseHsts();
}

webApplication.UseHttpsRedirection();
webApplication.UseStaticFiles();
webApplication.UseAntiforgery();

webApplication.MapRazorComponents<App>().AddInteractiveServerRenderMode();

webApplication.Run();
