using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using BowelMovementTracker.Data;
using BowelMovementTracker.Data.Enums;
using BowelMovementTracker.Data.Services.SecurityService;
using BowelMovementTracker.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BowelMovementTracker.Controllers;

public class AnalyticsController(BowelMovementTrackerContext context, IGuard securityService) : Controller
{
    [HttpGet("/{userid:guid?}/Calendar", Name = "AnalyticsCalendar"), Authorize]
    public async Task<IActionResult> Calendar([FromRoute] Guid? userid)
    {
        if (userid == null)
        {
            return NotFound();
        }
        
        var guardResults = securityService.ValidateOrRedirect(userid.Value);

        if (guardResults != null)
        {
            return guardResults;
        }

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
            UserIdentifier = userid.Value,
        };
        
        return View(viewModel);
    }

    [HttpPost("Analytics/UpdateDate")]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateDate([FromForm] string currentDate, [FromForm] string direction)
    {
        // Safely parse the exact format you are passing from the form
        var date = DateTime.ParseExact(currentDate, "MM/dd/yyyy", CultureInfo.InvariantCulture);
    
        DateTime newDate = direction switch
        {
            "back" => date.AddMonths(-1),
            "forward" => date.AddMonths(1),
            _ => DateTime.UtcNow
        };
    
        TempData["now"] = newDate.ToString("MM/dd/yyyy");
    
        return RedirectToRoute("AnalyticsCalendar", new { userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value });
    }

    [HttpGet("/{userid:guid}/Analytics", Name = "AnalyticsDashboard"), Authorize]
    public async Task<IActionResult> Index(
        [FromRoute] Guid? userid,
        [FromQuery(Name = "tPeriod")] int? tPeriod)
    {
        if (userid == null)
        {
            return NotFound();
        }
        
        var guardResults = securityService.ValidateOrRedirect(userid.Value);

        if (guardResults != null)
        {
            return guardResults;
        }

        int[] allowedTimeFilters = [7, 30, 365]; // Days

        if (tPeriod == null || !allowedTimeFilters.Contains(tPeriod.Value))
        {
            return RedirectToRoute("AnalyticsDashboard",
                new { userid = userid.Value, tPeriod = allowedTimeFilters[0] });
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
            UserIdentifier = userid.Value,
        };

        return View(viewModel);
    }
}