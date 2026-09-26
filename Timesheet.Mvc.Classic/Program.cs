using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Security.Claims;
using Timesheet.Core.Repositories;
using Timesheet.Core.Services;
using Timesheet.Core.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ITimesheetRepository, TimesheetRepository>();
builder.Services.AddScoped<ITimesheetService, TimesheetService>();
builder.Services.AddScoped<ITimesheetReportingService, TimesheetReportingService>();
builder.Services.AddScoped<ITimesheetEntryValidator, TimesheetEntryValidator>();

// Entra ID authentication configuration
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    // Make MVC treat "roles" as the role claim
    options.TokenValidationParameters.RoleClaimType = "roles";

    // Copy the "roles" claim from the ID token into the cookie identity
    options.ClaimActions.MapUniqueJsonKey("roles", "roles");
    options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;

});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("IsAdmin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("IsUser"));
});

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();
// END: Entra ID authentication configuration

var webApplication = builder.Build();

// Configure the HTTP request pipeline.
if (!webApplication.Environment.IsDevelopment())
{
    webApplication.UseExceptionHandler("/Home/Error");
    webApplication.UseHsts();
}

webApplication.UseHttpsRedirection();
webApplication.UseStaticFiles();
webApplication.UseRouting();
webApplication.UseAuthentication();   // first
webApplication.UseAuthorization();    // second

webApplication.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

webApplication.Run();