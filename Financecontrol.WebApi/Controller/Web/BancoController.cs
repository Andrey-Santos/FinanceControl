using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.Banco;
using FinanceControl.Core.Application.DTOs.Banco;

namespace Financecontrol.WebApi.Controllers.Web;

public class BancoController : Controller
{
    private readonly BancoUseCase _useCase;

    public BancoController(BancoUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _useCase.GetAllAsync());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BancoCreateDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _useCase.AddAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var tipo = await _useCase.GetByIdAsync(id);
        if (tipo == null)
            return NotFound();

        var dto = new BancoUpdateDto
        {
            Nome = tipo.Nome
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BancoUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _useCase.UpdateAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _useCase.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
