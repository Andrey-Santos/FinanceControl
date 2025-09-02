using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.DTOs.Login;
using FinanceControl.Core.Application.DTOs;

namespace FinanceControl.WebApi.Controllers.Web;

public class LoginController : Controller
{
    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Response.Cookies.Delete("jwt");
        return RedirectToAction("Index");
    }
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (HttpContext.Request.Cookies.ContainsKey("jwt"))
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (HttpContext.Request.Cookies.ContainsKey("jwt"))
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(LoginCreateDto dto)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri($"{Request.Scheme}://{Request.Host.Value}");
        var response = await client.PostAsJsonAsync("/api/Auth/create", dto);

        // if (response.IsSuccessStatusCode)
        //     return RedirectToAction("Index");

        TempData["Erro"] = await response.Content.ReadAsStringAsync();
        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Index(LoginRequestDto dto)
    {
        var client = _httpClientFactory.CreateClient();
        client.BaseAddress = new Uri($"{Request.Scheme}://{Request.Host.Value}");
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

        TempData["Erro"] = await response.Content.ReadAsStringAsync();
        return View(dto);
    }
}
