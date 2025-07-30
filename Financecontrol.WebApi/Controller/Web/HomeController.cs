using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
namespace FinanceControl.WebApi.Controllers;

[JwtAuthorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
