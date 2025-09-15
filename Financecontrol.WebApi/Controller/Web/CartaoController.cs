using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.Cartao;
using FinanceControl.Core.Application.DTOs.Cartao;
using FinanceControl.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using FinanceControl.Core.Domain.Enums;

namespace Financecontrol.WebApi.Controllers.Web;

public class CartaoController : Controller
{
    private readonly CartaoUseCase _useCase;

    private readonly IContaBancariaRepository _contasBancariasRepository;

    public CartaoController(CartaoUseCase useCase, IContaBancariaRepository contasBancariasRepository)
    {
        _useCase = useCase;
        _contasBancariasRepository = contasBancariasRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _useCase.GetAllAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var contas = await _contasBancariasRepository.GetAllAsync();

        ViewBag.Contas = new SelectList(contas, "Id", "Numero");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CartaoCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var contas = await _contasBancariasRepository.GetAllAsync();
            ViewBag.Contas = new SelectList(contas, "Id", "Numero");
            return View(dto);
        }

        await _useCase.AddAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var entity = await _useCase.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        var contas = await _contasBancariasRepository.GetAllAsync();

        ViewBag.Contas = new SelectList(contas, "Id", "Numero");

        var dto = new CartaoUpdateDto
        {
            Id = entity.Id,
            Apelido = entity.Apelido,
            ContaBancariaId = entity.ContaBancariaId,
            DiaFechamento = entity.DiaFechamento,
            Tipo = entity.Tipo,
            Limite = entity.Limite
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(CartaoUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            var contas = await _contasBancariasRepository.GetAllAsync();

            ViewBag.Contas = new SelectList(contas, "Id", "Numero");
            return View(dto);
        }

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
