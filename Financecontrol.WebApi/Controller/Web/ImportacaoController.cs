using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.Transacao;

using FinanceControl.Core.Application.DTOs.Transacao;
using FinanceControl.Core.Application.UseCases.ContaBancaria;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinanceControl.WebApi.Controllers;

[JwtAuthorize]
public class ImportacaoController : Controller
{

    private readonly TransacaoUseCase _transacaoUseCase;
    private readonly ContaBancariaUseCase _contaBancariaRepository;

    public ImportacaoController(TransacaoUseCase transacaoUseCase, ContaBancariaUseCase contaBancariaRepository)
    {
        _transacaoUseCase = transacaoUseCase;
        _contaBancariaRepository = contaBancariaRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        await LoadLists();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(TransacaoCreateDto transacao, IFormFile arquivo)
    {
        await LoadLists();
        try
        {
            var vQtdLinhasImportadas = await _transacaoUseCase.ImportarAsync(arquivo, transacao.ContaBancariaId);

            ViewBag.Sucesso = $"{vQtdLinhasImportadas} transações importadas com sucesso!";
            return View("Index", transacao);
        }
        catch (Exception ex)
        {
            ViewBag.Erro = $"Erro ao processar o arquivo: {ex.Message}";
            return View("Index", transacao);
        }
    }

    public async Task LoadLists()
    {
        var contas = await _contaBancariaRepository.GetAllAsync();

        ViewBag.Contas = new SelectList(contas, "Id", "Numero");
    }

}
