using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.Transacao;
using FinanceControl.Core.Application.DTOs.Transacao;
using FinanceControl.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using FinanceControl.Core.Domain.Enums;

namespace Financecontrol.WebApi.Controllers.Web;

public class TransacaoController : Controller
{
    private readonly TransacaoUseCase _useCase;
    private readonly IContaBancariaRepository _contaBancariaRepository;
    private readonly ICategoriaTransacaoRepository _categoriaTransacaoRepository;

    public TransacaoController(TransacaoUseCase useCase, IContaBancariaRepository contaBancariaRepository, ICategoriaTransacaoRepository categoriaTransacaoRepository)
    {
        _useCase = useCase;
        _contaBancariaRepository = contaBancariaRepository;
        _categoriaTransacaoRepository = categoriaTransacaoRepository;
    }

    public async Task LoadLists()
    {
        var contas = await _contaBancariaRepository.GetAllAsync();
        var categorias = await _categoriaTransacaoRepository.GetAllAsync();

        ViewBag.Tipos = new SelectList(Enum.GetValues(typeof(TipoTransacao)));
        ViewBag.Categorias = new SelectList(categorias, "Id", "Nome");
        ViewBag.Contas = new SelectList(contas, "Id", "Numero");
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var result = await _useCase.GetAllAsync();
        return View(result = result
                    .OrderByDescending(t => t.DataEfetivacao)
                    .ThenByDescending(t => t.Id));
    }

    [HttpGet]
    public async Task<IActionResult> Create(TipoTransacao? tipo)
    {
        await LoadLists();

        var model = new TransacaoCreateDto
        {
            Tipo = tipo ?? TipoTransacao.Despesa,
            DataEfetivacao = DateTime.Now
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TransacaoCreateDto dto, bool continuar = false)
    {
        if (!ModelState.IsValid)
        {
            await LoadLists();
            return View(dto);
        }

        await _useCase.AddAsync(dto);

        if (continuar)
        {
            TempData["MensagemSucesso"] = "Transação adicionada com sucesso!";
            return RedirectToAction("Create", new { tipo = dto.Tipo });
        }
        else
            return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var entity = await _useCase.GetByIdAsync(id);
        if (entity == null)
            return NotFound();

        await LoadLists();

        var dto = new TransacaoUpdateDto
        {
            Id = entity.Id,
            Descricao = entity.Descricao,
            DataEfetivacao = entity.DataEfetivacao,
            Valor = entity.Valor,
            ContaBancariaId = entity.ContaBancariaId,
            CategoriaId = entity.CategoriaId,
            Tipo = entity.Tipo
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TransacaoUpdateDto dto)
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
        return RedirectToAction("Index");
    }
}
