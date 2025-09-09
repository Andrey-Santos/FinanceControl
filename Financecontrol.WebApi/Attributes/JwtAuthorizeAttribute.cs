using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var isAuthenticated = context.HttpContext.User.Identity?.IsAuthenticated ?? false;

        if (!isAuthenticated)
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        // Garante que o token possui o NameIdentifier e já disponibiliza o UserId para os controllers
        var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
            return;
        }

        context.HttpContext.Items["UserId"] = userId;
    }
}
