using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class JwtAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var isAuthenticated = context.HttpContext.User.Identity?.IsAuthenticated ?? false;

        if (!isAuthenticated)
        {
            context.Result = new RedirectToActionResult("Index", "Login", null);
        }
    }
}
