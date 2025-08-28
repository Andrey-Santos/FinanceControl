using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.DTOs.Login;

namespace FinanceControl.WebApi.Controllers.Web;

public class LoginController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        if(HttpContext.Request.Cookies.ContainsKey("jwt"))
            return RedirectToAction("Index", "Home");
            
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginRequestDto dto)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("http://localhost:5020/api/Auth/login", dto);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

            if (result != null)
            {
                HttpContext.Response.Cookies.Append("jwt", result.Token);
                return RedirectToAction("Index", "Home");
            }
        }

        ViewBag.Erro = "Credenciais inválidas.";
        return View(dto);
    }
}
