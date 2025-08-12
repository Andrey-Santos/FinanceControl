using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.Transacao;
using FinanceControl.Core.Application.DTOs.Transacao;
using FinanceControl.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Financecontrol.WebApi.Controllers.Web;

public class TransacaoController : Controller
{
    private readonly TransacaoUseCase _useCase;

    private readonly IContaBancariaRepository _contaBancariaRepository;
    private readonly ITipoTransacaoRepository _tipoTransacaoRepository;

    public TransacaoController(TransacaoUseCase useCase, IContaBancariaRepository contaBancariaRepository, ITipoTransacaoRepository tipoTransacaoRepository)
    {
        _useCase = useCase;
        _contaBancariaRepository = contaBancariaRepository;
        _tipoTransacaoRepository = tipoTransacaoRepository;
    }

    public async Task Load()
    {
        var contas = await _contaBancariaRepository.GetAllAsync();
        var tipos = await _tipoTransacaoRepository.GetAllAsync();

        ViewBag.Tipos = new SelectList(tipos, "Id", "Nome");
        ViewBag.Contas = new SelectList(contas, "Id", "Numero");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _useCase.GetAllAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await Load();

        var model = new TransacaoCreateDto
        {
            DataEfetivacao = DateTime.Now
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransacaoCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            await Load();
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

        await Load();

        var dto = new TransacaoUpdateDto
        {
            Id = entity.Id,
            Descricao = entity.Descricao,
            DataEfetivacao = entity.DataEfetivacao,
            Valor = entity.Valor,
            ContaBancariaId = entity.ContaBancariaId,
            TipoId = entity.TipoId
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TransacaoUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            await Load();
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
        return RedirectToAction("Index");
    }
}
