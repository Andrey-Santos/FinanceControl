using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.DTOs.Login;

namespace FinanceControl.WebApi.Controllers;

public class LoginController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginRequestDto dto)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("http://localhost:5020/api/Auth/login", dto);

        if (response.IsSuccessStatusCode)
        {
            var token = await response.Content.ReadAsStringAsync();
            HttpContext.Response.Cookies.Append("jwt", token);
            return RedirectToAction("Index", "Home");
        }

        ViewBag.Erro = "Credenciais inválidas.";
        return View(dto);
    }
}
