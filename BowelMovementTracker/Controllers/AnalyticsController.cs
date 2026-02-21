using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BowelMovementTracker.Controllers;

public class AnalyticsController : Controller
{
    // GET
    [HttpGet("/Analytics"), AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
}