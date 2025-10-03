using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.ContaPagarReceber;
using FinanceControl.Core.Application.DTOs.ContaPagarReceber;
using FinanceControl.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using FinanceControl.Core.Domain.Enums;

namespace Financecontrol.WebApi.Controllers.Web;

public class ContaPagarReceberController : Controller
{
    private readonly ContaPagarReceberUseCase _useCase;
    private readonly IContaBancariaRepository _contasBancariasRepository;
    private readonly ICategoriaTransacaoRepository _categoriaTransacaoRepository;

    public ContaPagarReceberController(ContaPagarReceberUseCase useCase, IContaBancariaRepository contasBancariasRepository, ICategoriaTransacaoRepository categoriaTransacaoRepository)
    {
        _useCase = useCase;
        _contasBancariasRepository = contasBancariasRepository;
        _categoriaTransacaoRepository = categoriaTransacaoRepository;
    }

    public async Task LoadLists()
    {
        ViewBag.Categorias = new SelectList(await _categoriaTransacaoRepository.GetAllAsync(), "Id", "Nome");
        ViewBag.Contas     = new SelectList(await _contasBancariasRepository.GetAllAsync(), "Id", "Numero");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _useCase.GetAllAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadLists();

        var dto = new ContaPagarReceberCreateDto
        {
            Tipo = TipoTransacao.Despesa,
            DataVencimento = DateTime.Now.AddMonths(1),
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContaPagarReceberCreateDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadLists();
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

        await LoadLists();

        var dto = new ContaPagarReceberUpdateDto
        {
            Id = entity.Id,
            Descricao = entity.Descricao,
            Valor = entity.Valor,
            Tipo = entity.Tipo,
            CategoriaId = entity.CategoriaId,
            DataVencimento = entity.DataVencimento,
            DataPagamento = entity.DataPagamento ?? null,
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ContaPagarReceberUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            await LoadLists();
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pagar(int id, DateTime dataPagamento)
    {
        await _useCase.PagarAsync(id, dataPagamento);
        return RedirectToAction(nameof(Index));
    }
}
