using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
namespace FinanceControl.WebApi.Controllers;

[JwtAuthorize]
public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public HomeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }
}
