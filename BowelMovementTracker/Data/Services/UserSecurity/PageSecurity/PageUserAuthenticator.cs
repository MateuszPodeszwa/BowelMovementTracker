using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace BowelMovementTracker.Data.Services.UserSecurity.PageSecurity;

// Ensures that only "we" can enter the "our" page. For example, if user requests page of other user, it should validate the ownership.
public class PageUserAuthenticator(IHttpContextAccessor httpContextAccessor) : IGuard
{

    public IActionResult? ValidateOrRedirect(Guid userIdentifier)
    {
        var context = httpContextAccessor.HttpContext;

        // Ensure "we" are the web request.
        if (context == null)
        {
            throw new InvalidOperationException("PageUserAuthenticator cannot be used outside an HTTP Request.");
        }

        var user = context.User;

        // Is the user actually logged in?
        if (user?.Identity?.IsAuthenticated != true)
        {
            // They are anonymous (Ghost User). Send them to login.
            //return new RedirectToRouteResult("LoginRoute");

            // This should never happen if [Authorize] is used correctly.
            // Throw an error to alert the DEVELOPER that you forgot the attribute.
            throw new InvalidOperationException("PageUserAuthenticator requires an authenticated user.");
        }

        // Retrieve UID stored in the Cookie
        var loggedInUserIdStr = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Safety catch: if the cookie is malformed or missing the ID claim
        if (!Guid.TryParse(loggedInUserIdStr, out Guid loggedInUserId))
        {
            return new UnauthorizedResult();
        }

        // If an ID was provided in the URL, verify it matches the logged-in user
        if (userIdentifier != loggedInUserId)
        {
            return new ForbidResult(); // HTTP 403: They are trying to view someone else's content
        }

        return null;
    }
}