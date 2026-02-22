using System.Diagnostics;
using System.Security.Claims;
using BowelMovementTracker.Data;
using BowelMovementTracker.Data.Enums;
using BowelMovementTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BowelMovementTracker.Controllers;

public class AnalyticsController(BowelMovementTrackerContext context) : Controller
{
    [HttpGet("/{id:guid}/Analytics", Name = "AnalyticsDashboard"), Authorize]
    public async Task<IActionResult> Index(
        [FromRoute(Name = "id")] Guid? userid,
        [FromQuery(Name = "tPeriod")] int? tPeriod)
    {
        // Ensure only logged user can access their data
        // If the user ID from route matches the logged user ID, allow. Otherwise, restrict.
        // TODO: DRY: Encapsulate this (Reused from HomeController)
        // Retrieve the logged-in user's ID from the cookie claims
        var loggedInUserIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(loggedInUserIdStr, out Guid loggedInUserId))
        {
            // Safety catch: if the cookie is malformed or missing the ID claim
            return Unauthorized();
        }

        if (!userid.HasValue)
        {
            return NotFound();
        }

        // If an ID was provided in the URL, verify it matches the logged-in user
        if (userid.Value != loggedInUserId)
        {
            return Forbid(); // HTTP 403: They are trying to view someone else's dashboard
        }

        int[] allowedTimeFilters = [7, 30, 365]; // Days

        if (tPeriod == null || !allowedTimeFilters.Contains(tPeriod.Value))
        {
            return RedirectToRoute("AnalyticsDashboard", new { id = loggedInUserId, tPeriod = allowedTimeFilters[0] });
        }
        
        ViewData["RequestedTimePeriodInt"] = tPeriod.Value;
        ViewData["AllowedTimeFiltersArray"] = allowedTimeFilters;

        User? user = await context.User
            .Include(d => d.Diary)
            .ThenInclude(d => d.Logs)
            .FirstOrDefaultAsync(u => u.UserIdentifier == userid);

        if (user == null)
        {
            TempData["ErrorMessage"] = "Couldn't fetch user.";
            return View("Error",
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        var userLogsMapList = user.Diary.Logs.Select(log => new LogItemsViewModel
        {
            Identifier = log.LogIdentifier,
            BowelMovementType = log.LogBowelMovementType,
            DateTime = log.LogDateTime,
            LastUpdated = log.LogLastUpdated,
            WasCoffeeConsumed = log.LogWasCoffeeConsumed ?? false,
            WasMilkConsumed = log.LogWasMilkConsumed ?? false,
            Notes = log.LogNotes ?? ""
        }).ToList();


        var viewModel = new AnalyticsViewModel
        {
            AllLogItems = userLogsMapList,
            UserIdentifier = loggedInUserId,
        };

        return View(viewModel);
    }
}