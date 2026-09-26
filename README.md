# 📄 Timesheet System — Technical Test

## 💡 Overview

This technical test is designed to assess your coding skills, attention to detail, and problem-solving ability.  

You are asked to build a simplified timesheet system with a front-end interface relevant to CMap.

---

## ✅ Requirements

### Timesheet entry

Each timesheet entry should include:

- **User ID**  
- **Project ID**  
- **Date**  
- **Hours worked** (decimal, e.g., 7.5)  
- **Description** (optional)

---

### Core functionality

1. **Add a new timesheet entry**
2. **Edit an existing entry**
3. **Delete an entry**
4. **List all entries for a given user and week**
5. **Get total hours per project for a given user and week**

---

### Constraints

- Use **in-memory storage** only (e.g., lists, dictionaries). No database.
- Fully **unit tested**, covering core logic and edge cases.
- Implemented in **C# (.NET 6 or later)**.
- Include a **front-end** using either **Razor** or **Blazor** to enable testing of user interactions.
- Focus on clean, maintainable, and well-structured code.
- Prevent duplicate entries for the same user, project, and date.

---

## 📌 What we’re looking for

- Clean code and good design choices
- Proper handling of edge cases
- Comprehensive and meaningful unit tests
- Clear structure and readability


---
---
---
# Microsoft Entra ID Application Role‑Based Access Control (App Roles RBAC)

> Nothing to do with the Timesheet, just in this project  
> **NOTE:** C# MVC uses cookie-authentication  
> _(not JSON WEB TOKEN - JWT)_

Azure Portal → Microsoft Entra ID

### Create Azure Entra tenant
* Azure Portal → Microsoft Entra ID:
  Use existing tenant: Default Directory _(or create new)_

### Create test users
* Tenant → Add → user → create new user: 
 
**user@jonesstevelive.onmicrosoft.com**  
Display name: EntraTestUser  
PW: **4Candles**  
		
**admin@jonesstevelive.onmicrosoft.com**  
Display name: EntraTestAdmin  
PW: **Fork_Handles**

* Both users need → Entra ID → Manage → Users → Authentication Methods  
* Default sign‑in method: No default
* Usable authentication methods: No usable methods
* Non‑usable authentication methods: None
* System preferred MFA method: Enabled
* **NOTE** Had to login to Microsoft Entra Admin: **https://entra.microsoft.com/** and turn off 1st login PW change and also MFA for the 2 new users, to simplify setup.

### Register your MVC app	   
* Tenant → Add → App Registration:
		- Name: EntraTestMvcApp  
		- Supported account types: Single Tenant  
		- Redirect URI:  
					- Type: Web   
					- URI:   
					- (for local) https://localhost:7147/signin-oidc
					- (for live) https://cmaptimesheet-gff8embhgmbbh2ec.uksouth-01.azurewebsites.net/signin-oidc
					
					
### 	Add App Roles (RBAC)
* Manage → App registrations → EntraTestMvcApp → Manage → App roles → Create app role:  

Display name: RoleUser  
Value: IsUser  
Allowed member types: User  
Description: User role  

Display name: AdminUser  
Value: IsAdmin  
Allowed member types: User  
Description: Admin role  

### Assign roles to your test users
* Enterprise Applications → EntraTestMvcApp → Manage Users and groups → Add user/group → EntraTestUser → Select a Role → RoleUser → Assign
* Enterprise Applications → EntraTestMvcApp → Manage Users and groups → Add user/group → EntraTestAdmin → Select a Role → RoleAdmin → Assign  
_**(repeat to add admin also to Role: User)**_

To see users role Assignments:   
* Enterprise Applications → EntraTestMvcApp → Users and Groups

### Enable ID tokens in Azure for the App
* Azure Portal → Entra ID → App registrations → EntraTestMvcApp → Authentication → Settings  
Tick: Access tokens (used for implicit flows)  
Tick: ID tokens (used for implicit and hybrid flows)

---

## Setup the C# MVC App **Timesheet.Mvc.Classic** to implement Authorization:

### Add Authentication + Authorization (Program.cs)

Add using _(and Install via Nuget Package Manager where needed)_
```
using Microsoft.AspNetCore.Authentication;  
using Microsoft.AspNetCore.Authentication.OpenIdConnect;  
using Microsoft.Identity.Web;  
using Microsoft.Identity.Web.UI;  
using System.Security.Claims;  
```
Add this  code:
```
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
```
...and after `var webApplication = builder.Build();`
```
webApplication.UseAuthentication();   // first
webApplication.UseAuthorization();    // second
```

### Add AzureAd Settings in appsettings.json
``` 
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "jonesstevelive.onmicrosoft.com",
    "TenantId": "a2cee46e-8024-46ce-bc6f-a68fa1bc1ec0",
    "ClientId": "f1791820-ff60-421b-84e5-e58caa2201a5",
    "CallbackPath": "/signin-oidc"
  }

```
* 'Domain' and 'TenantId' from: Azure → Microsoft Entra ID → Tenants → Default Driectory  

* 'ClientId' from: Azure → Microsoft Entra ID → App registrations → Your MVC App → Overview → Application (client) ID
	
### Add Login + Logout Actions:  
 * Create AccountController.cs

```
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Timesheet.Mvc.Classic.Controllers;

public class AccountController : Controller
{
    public IActionResult SignIn()
    {
        return Challenge(
            new AuthenticationProperties { RedirectUri = "/" },
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    [Authorize]
    public IActionResult SignOut()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            OpenIdConnectDefaults.AuthenticationScheme,
            "Cookies");
    }
}

```

### Add Login/Logout Links 
* Views/Shared/_Layout.cshtml

```
<!-- Microsoft Entra ID -->
<div class="d-flex align-items-center gap-3">
@if (User.Identity.IsAuthenticated)
{
	<span class="fw-semibold">Hello @User.Identity.Name</span>
	<a asp-area="MicrosoftIdentity" asp-controller="Account" asp-action="SignOut" class="btn btn-outline-secondary btn-sm">Logout</a>
}
else
{
	<a asp-area="MicrosoftIdentity" asp-controller="Account" asp-action="SignIn" class="btn btn-primary btn-sm">Login</a>
}

<a asp-controller="Home" asp-action="AdminOnlyPage" class="link-secondary text-decoration-none">Admin Only Page</a>
<a asp-controller="Home" asp-action="UserOnlyPage" class="link-secondary text-decoration-none">User Only Page</a>
<a asp-controller="Home" asp-action="MyRoles" class="link-secondary text-decoration-none">My Roles</a>
</div>
```

### Protect Pages With Roles
 * HomeController.cs
```
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
```
* add view Home/AdminOnlyPage.cshtml
```
<h2>Admin Only Page</h2>
<p>
Has the Claim: <i>http://schemas.microsoft.com/ws/2008/06/identity/claims/role: IsUser</i>
</p>
```

* add view Home/UserOnlyPage.cshtml
```
<h2>User Only Page</h2>
<p>
Has the Claim: <i>http://schemas.microsoft.com/ws/2008/06/identity/claims/role: IsUser</i>
<br/>
(Admins can access this as they also have this Claim)
</p>
```

### Show Roles and Claims logged in user has
* new AccountController.cs

```
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Timesheet.Mvc.Classic.Controllers;

public class AccountController : Controller
{
    public IActionResult SignIn()
    {
        return Challenge(
            new AuthenticationProperties { RedirectUri = "/" },
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    [Authorize]
    public IActionResult SignOut()
    {
        return SignOut(
            new AuthenticationProperties { RedirectUri = "/" },
            OpenIdConnectDefaults.AuthenticationScheme,
            "Cookies");
    }
}
```

 * new Views/Home/MyRoles.cshtml
```
@using Timesheet.Mvc.Classic.Controllers
@model HomeController.MyRolesViewModel

<h2>Authentication Status</h2>
<p>Authenticated: <b>@Model.IsAuthenticated</b></p>

<h2>Claims</h2>
<ul>
    @foreach (var claim in Model.Claims)
    {
        <li><strong>@claim.Type</strong>: @claim.Value</li>
    }
</ul>
```

### Test Your Role Assignments
Log in as each test user and check:  

**Admin user:**  
Can access /AdminPage  
Can access /UserPage  
_UN:user@jonesstevelive.onmicrosoft.com / PW: 4Candles_

**Basic user:**  
Can access /UserPage  
Cannot access /AdminPage  
_UN: admin@jonesstevelive.onmicrosoft.com /PW: Fork_Handles_


**NOTE** STE:TODO: Redirecting a user after logout has not been setup.