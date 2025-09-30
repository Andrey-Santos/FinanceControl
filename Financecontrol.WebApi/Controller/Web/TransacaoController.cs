using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using FinanceControl.Core.Application.DTOs.Transacao;
using FinanceControl.Core.Application.Workflows;
using FinanceControl.Core.Domain.Enums;
using FinanceControl.Core.Domain.Interfaces;

namespace Financecontrol.WebApi.Controllers.Web;

public class TransacaoController : Controller
{
    private readonly FinanceWorkflowService _workflow;
    private readonly IContaBancariaRepository _contaBancariaRepository;
    private readonly ICategoriaTransacaoRepository _categoriaTransacaoRepository;
    private readonly ICartaoRepository _cartaoRepository;

    public TransacaoController(
        FinanceWorkflowService workflow,
        IContaBancariaRepository contaBancariaRepository,
        ICategoriaTransacaoRepository categoriaTransacaoRepository,
        ICartaoRepository cartaoRepository)
    {
        _workflow = workflow;
        _contaBancariaRepository = contaBancariaRepository;
        _categoriaTransacaoRepository = categoriaTransacaoRepository;
        _cartaoRepository = cartaoRepository;
    }

    private async Task LoadLists(TransacaoFilterDto? filtro = null)
    {
        ViewBag.Categorias = new SelectList(await _categoriaTransacaoRepository.GetAllAsync(), "Id", "Nome", filtro?.CategoriaId);
        ViewBag.Contas     = new SelectList(await _contaBancariaRepository.GetAllAsync(), "Id", "Numero", filtro?.ContaBancariaId);
    }

    [HttpGet]
    public async Task<IActionResult> Index(TransacaoFilterDto? filtro = null)
    {
        await LoadLists(filtro);

        var result = filtro is null
            ? await _workflow.GetAllTransacoesAsync()
            : await _workflow.GetFilteredTransacoesAsync(filtro);

        result = result
            .OrderByDescending(t => t.DataEfetivacao)
            .ThenByDescending(t => t.Id);

        ViewBag.Filtro = filtro ?? new TransacaoFilterDto();
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Create(TipoTransacao? tipo)
    {
        await LoadLists();

        var model = new TransacaoCreateDto
        {
            Tipo = tipo ?? TipoTransacao.Despesa,
            TipoOperacao = TipoOperacao.Debito,
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

        await _workflow.RegistrarTransacaoAsync(dto);

        if (continuar)
        {
            TempData["MensagemSucesso"] = "Transação adicionada com sucesso!";
            return RedirectToAction("Create", new { tipo = dto.Tipo, contaBancariaId = dto.ContaBancariaId, categoriaId = dto.CategoriaId });
        }

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> CartoesPorConta(long contaBancariaId, int tipoOperacao)
    {
        var cartoes = await _cartaoRepository.GetAllAsync();

        var filtrados = cartoes
            .Where(c => c.ContaBancariaId == contaBancariaId &&
                        c.Tipo == ((TipoOperacao)tipoOperacao == TipoOperacao.Credito ? TipoCartao.Credito : TipoCartao.Debito))
            .Select(c => new { id = c.Id, nome = c.Apelido })
            .ToList();

        return Json(filtrados);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var entity = await _workflow.GetTransacaoByIdAsync(id);
        if (entity == null)
            return NotFound();

        await LoadLists();

        var dto = new TransacaoUpdateDto(entity);
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

        await _workflow.UpdateTransacaoAsync(dto);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _workflow.DeleteTransacaoAsync(id);
        return RedirectToAction("Index");
    }
}
