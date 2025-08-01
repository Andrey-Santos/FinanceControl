using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.TipoTransacao;
using FinanceControl.Core.Application.DTOs.TipoTransacao;

namespace Financecontrol.WebApi.Controllers.Web;

public class TipoTransacaoController : Controller
{
    private readonly TipoTransacaoUseCase _useCase;

    public TipoTransacaoController(TipoTransacaoUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tipos = await _useCase.GetAllAsync();
        return View(tipos);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TipoTransacaoCreateDto dto)
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

        var dto = new TipoTransacaoUpdateDto
        {
            Nome = tipo.Nome
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TipoTransacaoUpdateDto dto)
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
