using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers.Web;

[JwtAuthorize]
public class ConfiguracaoController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
