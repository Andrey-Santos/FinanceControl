using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.Transacao;
using FinanceControl.Core.Domain.Enums;
using FinanceControl.Core.Domain.Entities;
using System.Security.Claims;

namespace FinanceControl.WebApi.Controllers;

[JwtAuthorize]
public class HomeController : Controller
{
    private readonly TransacaoUseCase _transacaoUseCase;

    public HomeController(TransacaoUseCase transacaoUseCase)
    {
        _transacaoUseCase = transacaoUseCase;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? mes, int? ano)
    {
         var usuarioIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(usuarioIdClaim) || !long.TryParse(usuarioIdClaim, out var usuarioId))
            return RedirectToAction("Index", "Login");

        var mesAtual = mes ?? DateTime.Today.Month;
        var anoAtual = ano ?? DateTime.Today.Year;

        var (categorias, despesas, receitas, saldoAtual, saldoMesAnterior, saldoPrevistoProximoMes) = await _transacaoUseCase.GetResumoDashboardAsync(mesAtual, anoAtual, usuarioId);

        ViewBag.GraficoCategoriasLabels = categorias.Select(c => c.Categoria).ToList();
        ViewBag.GraficoCategoriasValores = categorias.Select(c => Math.Abs(c.Valor)).ToList();
        var cores = new[] { "#007bff", "#28a745", "#ffc107", "#dc3545", "#6f42c1", "#20c997", "#fd7e14", "#17a2b8", "#343a40", "#6610f2" };
        ViewBag.GraficoCategoriasCores = categorias.Select((c, i) => cores[i % cores.Length]).ToList();

        ViewBag.GraficoDespesasLabels = despesas.Select(c => c.Categoria).ToList();
        ViewBag.GraficoDespesasValores = despesas.Select(c => Math.Abs(c.Valor)).ToList();
        var coresDespesas = new[] { "#007bff", "#28a745", "#ffc107", "#dc3545", "#6f42c1", "#20c997", "#fd7e14", "#17a2b8", "#343a40", "#6610f2" };
        ViewBag.GraficoDespesasCores = despesas.Select((c, i) => coresDespesas[i % coresDespesas.Length]).ToList();

        ViewBag.GraficoReceitasLabels = receitas.Select(c => c.Categoria).ToList();
        ViewBag.GraficoReceitasValores = receitas.Select(c => Math.Abs(c.Valor)).ToList();
        var coresReceitas = new[] { "#007bff", "#28a745", "#ffc107", "#dc3545", "#6f42c1", "#20c997", "#fd7e14", "#17a2b8", "#343a40", "#6610f2" };
        ViewBag.GraficoReceitasCores = receitas.Select((c, i) => coresReceitas[i % coresReceitas.Length]).ToList();

        ViewBag.SaldoAtual = saldoAtual;
        ViewBag.SaldoMesAnterior = saldoMesAnterior;
        ViewBag.SaldoPrevistoProximoMes = saldoPrevistoProximoMes;

        ViewBag.MesAtual = mesAtual;
        ViewBag.AnoAtual = anoAtual;

        // Se ainda quiser listar transações, carregue só do mês:
        var transacoesMes = await _transacaoUseCase.GetByFiltroAsync(mesAtual, anoAtual, usuarioId);
        return View(transacoesMes.OrderByDescending(t => t.DataEfetivacao).ThenByDescending(t => t.Id));
    }
}