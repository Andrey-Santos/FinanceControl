using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.Transacao;
using FinanceControl.Core.Application.DTOs.Transacao;
using FinanceControl.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using FinanceControl.Core.Domain.Enums;
using FinanceControl.Core.Domain.Entities;

namespace Financecontrol.WebApi.Controllers.Web;

public class TransacaoController : Controller
{
    private readonly TransacaoUseCase _useCase;
    private readonly IContaBancariaRepository _contaBancariaRepository;
    private readonly ICategoriaTransacaoRepository _categoriaTransacaoRepository;
    private readonly ICartaoRepository _cartaoRepository;

    public TransacaoController(TransacaoUseCase useCase, IContaBancariaRepository contaBancariaRepository, ICategoriaTransacaoRepository categoriaTransacaoRepository, ICartaoRepository cartaoRepository)
    {
        _useCase = useCase;
        _contaBancariaRepository = contaBancariaRepository;
        _categoriaTransacaoRepository = categoriaTransacaoRepository;
        _cartaoRepository = cartaoRepository;
    }

    public async Task LoadLists(TransacaoFilterDto? filtro = null)
    {
        var contas = await _contaBancariaRepository.GetAllAsync();
        var categorias = await _categoriaTransacaoRepository.GetAllAsync();

        ViewBag.Tipos = new SelectList(Enum.GetValues<TipoTransacao>(), filtro.Tipo);
        ViewBag.TiposOperacao = new SelectList(Enum.GetValues<TipoOperacao>(), filtro.TipoOperacao);
        ViewBag.Categorias = new SelectList(categorias, "Id", "Nome", filtro.CategoriaId);
        ViewBag.Contas = new SelectList(contas, "Id", "Numero", filtro.ContaBancariaId);
    }

    [HttpGet]
    public async Task<IActionResult> Index(TransacaoFilterDto? filtro = null)
    {
        await LoadLists(filtro);
        
        var result = await _useCase.GetAllAsync();
        
        if (filtro != null)
        {
            result = await _useCase.GetFilteredAsync(filtro);
        }
        
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

    [HttpGet]
    public async Task<IActionResult> CartoesPorConta(long contaBancariaId)
    {
        var cartoes = await _cartaoRepository.GetAllAsync();
        var filtrados = cartoes
            .Where(c => c.Tipo == TipoCartao.Credito && c.ContaBancariaId == contaBancariaId)
            .Select(c => new { id = c.Id, nome = c.Apelido })
            .ToList();
        return Json(filtrados);
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
            return RedirectToAction("Create", new { tipo = dto.Tipo, contaBancariaId = dto.ContaBancariaId, categoriaId = dto.CategoriaId});
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
