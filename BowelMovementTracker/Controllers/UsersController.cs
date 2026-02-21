using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BowelMovementTracker.Data;
using BowelMovementTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace BowelMovementTracker.Controllers;

[Authorize] // Secures all actions in this controller by default
public class UsersController(BowelMovementTrackerContext context) : Controller
{
    [AllowAnonymous] // Allows unauthenticated users to see the login page
    [HttpGet("/Login", Name = "LoginRoute")]
    public IActionResult Login()
    {
        var isUserLoggedIn = User.Identity is { IsAuthenticated: true };
        if (isUserLoggedIn) return Redirect("/");
        
        return View();
    }

    [AllowAnonymous]
    [HttpPost("/Login", Name = "LoginRoute"), ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginUser(
        [Bind("UserEmailAddress,UserPasswordHash"), FromForm]
        LoginViewModel boundUser)
    {
        // If using the dedicated LoginViewModel, you shouldn't need to remove "Diary" 
        // from ModelState, but I have kept your validation flow intact.
        ModelState.Remove("Diary");

        if (!ModelState.IsValid) return View("Login", boundUser);

        User? user = await context.User.FirstOrDefaultAsync(m =>
            (m.UserEmailAddress == boundUser.UserEmailAddress) &&
            (m.UserPasswordHash == boundUser.UserPasswordHash));

        if (user == null)
        {
            // It is safer to use a generic error message to prevent email enumeration
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View("Login", boundUser);
        }

        // --- AUTHENTICATION LOGIC ---
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserIdentifier.ToString()),
            new Claim(ClaimTypes.Email, user.UserEmailAddress),
            new Claim(ClaimTypes.Name, user.UserEmailAddress)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // This issues the authentication cookie to the user's browser
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        // -----------------------------

        return RedirectToRoute("UserHome", new { id = user.UserIdentifier });
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    [HttpGet("/Register"), AllowAnonymous]
    public IActionResult Register()
    {
        var isUserLoggedIn = User.Identity is { IsAuthenticated: true };
        if (isUserLoggedIn) return Redirect("/");

        return View();
    }

    [HttpPost, AllowAnonymous, ValidateAntiForgeryToken, ActionName("RegisterUser")]
    public async Task<IActionResult> RegisterUser(
        [FromForm, Bind("UserEmailAddress,UserPasswordHash")]
        LoginViewModel userRegisterData)
    {
        // Remove Diary to avoid problems
        ModelState.Remove("Diary");
        
        // Check if there's any User with that eMail
        var user = await context.User.FirstOrDefaultAsync(user =>
            user.UserEmailAddress == userRegisterData.UserEmailAddress);

        if (user != null)
        {
            ModelState.AddModelError("UserEmailAddress", "Email is already in use.");
            return View("Register", userRegisterData);
        }
        
        // Restrict Registration to specific user addresses, to ensure a specific group of people can register.
        // This hardcoded functionality is temporary.
        string[] userMail = [
            "podinatubie@gmail.com",
            "testmail@test.test"
        ];

        var any = userMail.Any(mail => mail == userRegisterData.UserEmailAddress);

        if (any)
        {
            ModelState.AddModelError(string.Empty, "Illegal Operation.");
        }
        
        // Return all issues before creating Database Object
        if (!ModelState.IsValid) return View("Register", userRegisterData);

        User userDbObject = new()
        {
            UserEmailAddress = userRegisterData.UserEmailAddress,
            UserPasswordHash = userRegisterData.UserPasswordHash,

            Diary = new()
            {
                // Assign null! to satisfy the C# compiler's 'required' rule 
                // without throwing a nullability warning.
                User = null!,
                // ReSharper disable once PreferConcreteValueOverDefault
                DiaryUserIdentifier = default,
                Logs = new List<Log>()
            }
        };

        context.User.Add(userDbObject);
        await context.SaveChangesAsync();

        // === AUTO LOGIN LOGIC ===
        // Create the claims using the newly saved userDbObject.
        // Entity Framework will have automatically populated the UserIdentifier 
        // after SaveChangesAsync() if it is a database-generated key.
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userDbObject.UserIdentifier.ToString()),
            new Claim(ClaimTypes.Email, userDbObject.UserEmailAddress),
            new Claim(ClaimTypes.Name, userDbObject.UserEmailAddress)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // Issue the authentication cookie
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        // Redirect directly to the user's home page instead of the login page
        return RedirectToRoute("UserHome", new { id = userDbObject.UserIdentifier });
    }

    private bool UserExists(Guid id)
    {
        return context.User.Any(e => e.UserIdentifier == id);
    }
}