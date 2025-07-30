using Microsoft.AspNetCore.Mvc;
namespace FinanceControl.WebApi.Controllers;

[JwtAuthorize]
public class ConfiguracaoController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ConfiguracaoController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }
}
