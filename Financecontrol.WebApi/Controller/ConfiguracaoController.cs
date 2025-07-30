using Microsoft.AspNetCore.Mvc;
namespace FinanceControl.WebApi.Controllers;

[JwtAuthorize]
public class ConfiguracaoController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
