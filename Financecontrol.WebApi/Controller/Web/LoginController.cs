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
        if (HttpContext.Request.Cookies.ContainsKey("jwt"))
            return RedirectToAction("Index", "Home");

        return View();
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(LoginCreateDto dto)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/Auth/register", dto);

        if (response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        ViewBag.Erro = "Não foi possível criar a conta.";
        return View(dto);
    }
    public async Task<IActionResult> Index(LoginRequestDto dto)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/Auth/login", dto);

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
