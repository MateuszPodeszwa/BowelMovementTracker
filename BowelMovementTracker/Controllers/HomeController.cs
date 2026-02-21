using BowelMovementTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using BowelMovementTracker.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace BowelMovementTracker.Controllers;

public class HomeController(BowelMovementTrackerContext context) : Controller
{
    // Custom Routing Templates
    [
        Authorize,
        HttpGet("/"), HttpGet("/{id:guid}", Name = "UserHome"),     // Matches all traffic from the Route Base Level
        HttpGet("/Home/Index"), HttpGet("/Home/Index/{id:guid}")    // For Legacy & Compatibility Only (.../Home/Index/...)
    ]
    public async Task<IActionResult> Index
    (
        [FromRoute(Name = "id")] Guid? userIdentifier,
        [FromQuery(Name = "date")] DateTime? requestedDate
    )
    {
        // Authorization and Security
        // Retrieve the logged-in user's ID from the cookie claims
        var loggedInUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (!Guid.TryParse(loggedInUserIdStr, out Guid loggedInUserId))
        {
            // Safety catch: if the cookie is malformed or missing the ID claim
            return Unauthorized(); 
        }
        
        if (!userIdentifier.HasValue)
        {
            // Preserve the requestedDate query parameter if they provided one
            return RedirectToRoute("UserHome", new { id = loggedInUserId, date = requestedDate });
        }
        
        // If an ID was provided in the URL, verify it matches the logged-in user
        if (userIdentifier.Value != loggedInUserId)
        {
            return Forbid(); // HTTP 403: They are trying to view someone else's dashboard
        }
        
        // Ensure there's always a date parameter in URL - Get Current UTC
        if (!requestedDate.HasValue) return Redirect($"/{userIdentifier.Value}?date={DateTime.UtcNow:yyyy-MM-dd}");
        
        // Ensure the requestedDate has Time. For the logging functionality.
        if (requestedDate!.Value.TimeOfDay == TimeSpan.Zero)
            requestedDate = requestedDate.Value.Add(DateTime.Now.TimeOfDay);
        
        // Bag the requested date for view.
        ViewData["RequestedDate"] = requestedDate;

        // Get user information including their logs
        User? user = await context.User
            .Include(d => d.Diary)
            .ThenInclude(d => d.Logs)
            .FirstOrDefaultAsync(u => u.UserIdentifier == userIdentifier);

        // Validate userIdentifier exist.
        if (user == null)
        {
            // Send it in TempData for UI Error Message
            TempData["ErrorMessage"] = "Supplied User Identifier may be incorrect or couldn't be found. Please ensure that the ID parameter is correct.";
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Build the HomeViewModel for View
        var viewModel = new HomeViewModel
        {
            UserIdentifier = user.UserIdentifier,
            TotalLogCount = user.Diary?.Logs?.Count ?? 0,
            LogItems = user.Diary?.Logs?.Select(log => new LogItemsViewModel()
            {
                Identifier = log.LogIdentifier,
                BowelMovementType = log.LogBowelMovementType,
                DateTime = log.LogDateTime,
                LastUpdated = log.LogLastUpdated,
                WasCoffeeConsumed = log.LogWasCoffeeConsumed ?? false,
                WasMilkConsumed = log.LogWasMilkConsumed ?? false,
                Notes = log.LogNotes,

            }).Where(item => item.DateTime!.Value.Date == requestedDate.Value.Date).OrderByDescending(property => property.DateTime).ToList() ?? [],
        };
        
        return View(viewModel);
        
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }
    
    [AllowAnonymous, ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // Form sends userIdentifier as the asp-net-routeid to ensure right user gets updated.
    [HttpPost("Home/create-log/{id:guid}/{logid:guid?}")]
    [Authorize, ValidateAntiForgeryToken, ActionName("create-log")]
    // The Form on the Home/Index page can Update/Delete/Create
    // TODO: Implement Delete/Update functionalities.
    public async Task<IActionResult> UpdateLogEntry(
        [FromRoute(Name = "id")] Guid userIdentifier,
        [FromRoute(Name = "logid")] Guid? logIdentifier, // Used only for Updating & Deleting
        [FromForm] string submitAction,
        [Bind("BowelMovementType,DateTime,WasCoffeeConsumed,WasMilkConsumed,Notes")] LogItemsViewModel model
        // DateTime is not as important here, it can be edited by user. Due to the design circumstances.
        // The Date Picker input is hidden, and instead the input take the date URL parameter.
        // The LOG Identifier is handled by database server.
    )
    {
        if (!ModelState.IsValid) return RedirectToRoute("UserHome", new { id = userIdentifier });
        
        // Get User and their Logs
        User? user = await context.User
            .Include(d => d.Diary)
            .ThenInclude(d => d.Logs)
            .FirstOrDefaultAsync(u => u.UserIdentifier == userIdentifier);
        
        // In the very rare occasions. The Diary must be created with the User Object.
        if (user?.Diary == null) return NotFound("User or Diary not found.");
            
        // Find a specific log to delete/update, that is equal to logIdentifier and belongs to userIdentifier
        Log? log = user.Diary.Logs?.FirstOrDefault(log => log.LogIdentifier == logIdentifier);

        switch (submitAction?.ToLower().Trim())
        {
            case "delete":
            {
                if (log == null) return NotFound("Log not found.");

                context.Remove(log);
                await context.SaveChangesAsync();

                return RedirectToRoute("UserHome", new { id = userIdentifier });
            }
            case "edit":
            {
                // TODO: Implement more sophisticated error handling.
                if (log == null) return NotFound("Log not found.");

                log.LogBowelMovementType = model.BowelMovementType;
                log.LogWasCoffeeConsumed = model.WasCoffeeConsumed;
                log.LogWasMilkConsumed = model.WasMilkConsumed;
                log.LogNotes = model.Notes;
                log.LogLastUpdated = DateTime.UtcNow;

                context.Update(log);
                await context.SaveChangesAsync();

                return RedirectToRoute("UserHome", new { 
                    id = userIdentifier, 
                    date = log?.LogDateTime 
                });
            }
            case "log":
            {
                // Manually Map the LOG Object. Mind the DateTime fields.
                var newLog = new Log
                {
                    LogBowelMovementType = model.BowelMovementType,
                    LogWasCoffeeConsumed = model.WasCoffeeConsumed,
                    LogWasMilkConsumed = model.WasMilkConsumed,
                    LogNotes = model.Notes,

                    LogDateTime =
                        model.DateTime ??
                        DateTime.UtcNow, // Log Creation Date (Cannot be changed on this side, only UI - It marks the log date from URL Date property)
                    LogLastUpdated =
                        DateTime.UtcNow, // Updatable each time on change, mainly for the Log-Logging to the system.

                    // Because of the relationships, these must be supplied by query user object.
                    Diary = user.Diary,
                    LogDiaryIdentifier = user.Diary.DiaryIdentifier,
                };

                context.Add(newLog);
                await context.SaveChangesAsync();
            
                // Return to exactly the same page.
                // TODO: Consider implementing TempData with information that the LOG has been create/updated/deleted - Will be displayed as UI component.
                return RedirectToRoute("UserHome", new { 
                    id = userIdentifier, 
                    date = newLog.LogDateTime?.ToString("yyyy-MM-dd") 
                });
            }
        }
        
        return BadRequest("Invalid action for an existing log.");
    }
}