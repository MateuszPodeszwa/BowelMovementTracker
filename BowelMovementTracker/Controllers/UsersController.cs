using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BowelMovementTracker.Data;
using BowelMovementTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace BowelMovementTracker.Controllers;

[Authorize] // Secures all actions in this controller by default
public class UsersController(BowelMovementTrackerContext context) : Controller
{
    [AllowAnonymous] // Allows unauthenticated users to see the login page
    [HttpGet("/Login", Name = "LoginRoute")] 
    public IActionResult Login() => View();

    [AllowAnonymous]
    [HttpPost("/Login", Name = "LoginRoute"), ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginUser([Bind("UserEmailAddress,UserPasswordHash"), FromForm] LoginViewModel boundUser)
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

    private bool UserExists(Guid id)
    {
        return context.User.Any(e => e.UserIdentifier == id);
    }
}