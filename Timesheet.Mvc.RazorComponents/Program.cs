using Timesheet.Core.Repositories;
using Timesheet.Core.Services;
using Timesheet.Core.Validation;
using Timesheet.Mvc.RazorComponents.Components;

var webApplicationBuilder = WebApplication.CreateBuilder(args);

// Add services to the container.
webApplicationBuilder.Services.AddControllersWithViews();
webApplicationBuilder.Services.AddSingleton<ITimesheetRepository, TimesheetRepository>();
webApplicationBuilder.Services.AddScoped<ITimesheetService, TimesheetService>();
webApplicationBuilder.Services.AddScoped<ITimesheetReportingService, TimesheetReportingService>();
webApplicationBuilder.Services.AddScoped<ITimesheetEntryValidator, TimesheetEntryValidator>();
webApplicationBuilder.Services.AddRazorComponents().AddInteractiveServerComponents();



var webApplication = webApplicationBuilder.Build();

// Configure the HTTP request pipeline.
if (!webApplication.Environment.IsDevelopment())
{
    webApplication.UseExceptionHandler("/Home/Error");
    webApplication.UseHsts();
}

webApplication.UseHttpsRedirection();
webApplication.UseStaticFiles();
webApplication.UseRouting();
webApplication.UseAntiforgery();        // ⭐ REQUIRED for Razor Components
webApplication.MapBlazorHub();          // ⭐ REQUIRED for Razor Components
// No MapRazorComponents<App>(): it maps _framework/blazor.server.js a second time (AmbiguousMatchException, so Save never worked).
// The components live inside MVC views (<component render-mode="ServerPrerendered">), so the hub alone is enough.

webApplication.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

webApplication.Run();