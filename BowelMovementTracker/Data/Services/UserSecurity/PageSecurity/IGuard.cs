using Microsoft.AspNetCore.Mvc;

namespace BowelMovementTracker.Data.Services.UserSecurity.PageSecurity;

public interface IGuard
{
    IActionResult? ValidateOrRedirect(Guid userId);
}