using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.Transacao;
using FinanceControl.Core.Domain.Enums;
using FinanceControl.Core.Domain.Entities;
using System.Security.Claims;
using FinanceControl.Core.Application.UseCases.ContaPagarReceber;

namespace FinanceControl.WebApi.Controllers;

[JwtAuthorize]
public class HomeController : Controller
{
    private readonly TransacaoUseCase _transacaoUseCase;
    private readonly ContaPagarReceberUseCase _contaPagarReceberUseCase;

    public HomeController(TransacaoUseCase transacaoUseCase, ContaPagarReceberUseCase contaPagarReceberUseCase)
    {
        _transacaoUseCase = transacaoUseCase;
        _contaPagarReceberUseCase = contaPagarReceberUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? mes, int? ano)
    {
        var userIdObj = HttpContext.Items["UserId"];
        if (userIdObj is not long usuarioId || usuarioId == 0)
            return RedirectToAction("Index", "Login");

        var mesAtual = mes ?? DateTime.Today.Month;
        var anoAtual = ano ?? DateTime.Today.Year;

        await OnLoad(mesAtual, anoAtual, usuarioId);

        return View(ViewBag.TransacoesMes);
    }

    public async Task OnLoad(int mes, int ano, long usuarioId)
    {
        var (categorias, despesas, receitas, saldoAtual, saldoMesAnterior, saldoPrevistoProximoMes) =
            await _transacaoUseCase.GetResumoDashboardAsync(mes, ano, usuarioId);

        // Gráficos de categorias
        ViewBag.GraficoCategoriasLabels = categorias.Select(c => c.Categoria).ToList();
        ViewBag.GraficoCategoriasValores = categorias.Select(c => Math.Abs(c.Valor)).ToList();
        var cores = new[] { "#007bff", "#28a745", "#ffc107", "#dc3545", "#6f42c1", "#20c997", "#fd7e14", "#17a2b8", "#343a40", "#6610f2" };
        ViewBag.GraficoCategoriasCores = categorias.Select((c, i) => cores[i % cores.Length]).ToList();

        // Gráficos de despesas
        ViewBag.GraficoDespesasLabels = despesas.Select(c => c.Categoria).ToList();
        ViewBag.GraficoDespesasValores = despesas.Select(c => Math.Abs(c.Valor)).ToList();
        ViewBag.GraficoDespesasCores = despesas.Select((c, i) => cores[i % cores.Length]).ToList();

        // Gráficos de receitas
        ViewBag.GraficoReceitasLabels = receitas.Select(c => c.Categoria).ToList();
        ViewBag.GraficoReceitasValores = receitas.Select(c => Math.Abs(c.Valor)).ToList();
        ViewBag.GraficoReceitasCores = receitas.Select((c, i) => cores[i % cores.Length]).ToList();

        // Totalizadores
        ViewBag.SaldoAtual = saldoAtual;
        ViewBag.SaldoMesAnterior = saldoMesAnterior;

        ViewBag.SaldoPrevistoProximoMes = saldoPrevistoProximoMes;

        ViewBag.MesAtual = mes;
        ViewBag.AnoAtual = ano;

        var transacoesMes = await _transacaoUseCase.GetByFiltroAsync(mes, ano, usuarioId);
        ViewBag.TransacoesMes = transacoesMes
            .OrderByDescending(t => t.DataEfetivacao)
            .ThenByDescending(t => t.Id);
    }
}
