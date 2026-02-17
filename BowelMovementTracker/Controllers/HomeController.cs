using BowelMovementTracker.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using BowelMovementTracker.Data;
using BowelMovementTracker.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace BowelMovementTracker.Controllers;

public class HomeController(BowelMovementTrackerContext context) : Controller
{
    // Custom Routing Templates
    [
        HttpGet("/{id:guid}", Name = "UserHome"),
        HttpGet("/Home/Index"), 
        HttpGet("/Home/Index/{id:guid}"), 
        HttpGet("/")
    ]
    public async Task<IActionResult> Index
    (
        [FromRoute(Name = "id")] Guid? userIdentifier,
        [FromQuery(Name = "date")] DateTime? requestedDate
    )
    {
        // TODO: A more sophisticated check is required. Consider implementing authentication and authorization.
        if (!userIdentifier.HasValue) return RedirectToAction("Index", "Users");
        // Ensure there's always a date parameter in URL - Get Current UTC
        if (!requestedDate.HasValue) return Redirect($"/{userIdentifier.Value}?date={DateTime.UtcNow:yyyy-MM-dd}");
        // Ensure the requestedDate has Time. For the logging functionality.
        if (requestedDate!.Value.TimeOfDay == TimeSpan.Zero) requestedDate = requestedDate.Value.Add(DateTime.Now.TimeOfDay);
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
            TempData["ErrorMessage"] = "User not found";
            return View(new HomeViewModel
            {
                LogItems = []
            });
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
                WasMilkConsumed =  log.LogWasMilkConsumed ?? false,
                Notes = log.LogNotes,
                
            }).OrderByDescending(l => l.LastUpdated).ToList() ?? [],
        };
        
        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    // Form sends userIdentifier as the asp-net-routeid to ensure right user gets updated.
    [HttpPost("Home/create-log/{id:guid}")]
    [ValidateAntiForgeryToken, ActionName("create-log")]
    // The Form on the Home/Index page can Update/Delete/Create
    // TODO: Implement Delete/Update functionalities.
    public async Task<IActionResult> UpdateLogEntry(
        [FromRoute(Name = "id")] Guid userIdentifier,
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
        
        // Manually Map the LOG Object. Mind the DateTime fields.
        var newLog = new Log
        {
            LogBowelMovementType = model.BowelMovementType,
            LogWasCoffeeConsumed = model.WasCoffeeConsumed,
            LogWasMilkConsumed = model.WasMilkConsumed,
            LogNotes = model.Notes,
            
            LogDateTime = model.DateTime,       // Log Creation Date (Cannot be changed on this side, only UI - It marks the log date from URL Date property)
            LogLastUpdated = DateTime.UtcNow,   // Updatable each time on change, mainly for the Log-Logging to the system.
            
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