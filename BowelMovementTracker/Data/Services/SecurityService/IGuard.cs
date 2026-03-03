using Microsoft.AspNetCore.Mvc;

namespace BowelMovementTracker.Data.Services.SecurityService;

public interface IGuard
{
    IActionResult? ValidateOrRedirect(Guid userId);
}