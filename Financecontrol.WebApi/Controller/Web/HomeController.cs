using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.Transacao;
using FinanceControl.Core.Domain.Enums;
using FinanceControl.Core.Domain.Entities;

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
        var usuarioId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var mesAtual = mes ?? DateTime.Today.Month;
        var anoAtual = ano ?? DateTime.Today.Year;

        var inicioMesAtual = new DateTime(anoAtual, mesAtual, 1);
        var fimMesAnterior = inicioMesAtual.AddDays(-1);
        var fimProximoMes = inicioMesAtual.AddMonths(2).AddDays(-1);

        var transacoes = await _transacaoUseCase.GetAllAsync();
        // Filtra transações do mês selecionado
        var fimMesAtual = inicioMesAtual.AddMonths(1).AddDays(-1);
        var transacoesMes = transacoes.Where(t => t.DataEfetivacao >= inicioMesAtual && t.DataEfetivacao <= fimMesAtual).ToList();

        // Agrupa por nome da categoria
        var categorias = transacoesMes.GroupBy(t => t.CategoriaNome)
            .Select(g => new
            {
                UsuarioId = usuarioId,
                Categoria = g.Key,
                Valor = g.Sum(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor)
            })
            .Where(c => c.Valor != 0)
            .ToList();

        var despesas = transacoesMes.GroupBy(t => t.CategoriaNome)
            .Select(g => new
            {
                UsuarioId = usuarioId,
                Categoria = g.Key,
                Valor = g.Sum(t => t.Tipo == TipoTransacao.Despesa ? t.Valor : 0)
            })
            .Where(c => c.Valor != 0)
            .ToList();

        var receitas = transacoesMes.GroupBy(t => t.CategoriaNome)
            .Select(g => new
            {
                UsuarioId = usuarioId,
                Categoria = g.Key,
                Valor = g.Sum(t => t.Tipo == TipoTransacao.Receita ? t.Valor : 0)
            })
            .Where(c => c.Valor != 0)
            .ToList();

        // Prepara dados para o gráfico
        ViewBag.GraficoCategoriasLabels = categorias.Select(c => c.Categoria).ToList();
        ViewBag.GraficoCategoriasValores = categorias.Select(c => Math.Abs(c.Valor)).ToList();
        var cores = new[] { "#007bff", "#28a745", "#ffc107", "#dc3545", "#6f42c1", "#20c997", "#fd7e14", "#17a2b8", "#343a40", "#6610f2" };
        ViewBag.GraficoCategoriasCores = categorias.Select((c, i) => cores[i % cores.Length]).ToList();

        ViewBag.GraficoDespesasLabels = despesas.Select(c => c.Categoria).ToList();
        ViewBag.GraficoDespesasValores = despesas.Select(c => Math.Abs(c.Valor)).ToList();
        var coresDespesas = new[] { "#007bff", "#28a745", "#ffc107", "#dc3545", "#6f42c1", "#20c997", "#fd7e14", "#17a2b8", "#343a40", "#6610f2" };
        ViewBag.GraficoDespesasCores = despesas.Select((c, i) => coresDespesas[i % coresDespesas.Length]).ToList();

        ViewBag.GraficoReceitasLabels = receitas.Select(c => c.Categoria).ToList();
        ViewBag.GraficoReceitasValores = despesas.Select(c => Math.Abs(c.Valor)).ToList();
        var coresReceitas = new[] { "#007bff", "#28a745", "#ffc107", "#dc3545", "#6f42c1", "#20c997", "#fd7e14", "#17a2b8", "#343a40", "#6610f2" };
        ViewBag.GraficoReceitasCores = despesas.Select((c, i) => coresReceitas[i % coresReceitas.Length]).ToList();

        ViewBag.SaldoAtual = transacoes
                                .Where(t => t.DataEfetivacao <= inicioMesAtual.AddMonths(1).AddDays(-1))
                                .Sum(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);

        ViewBag.SaldoMesAnterior = transacoes
                                .Where(t => t.DataEfetivacao <= fimMesAnterior)
                                .Sum(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);

        ViewBag.SaldoPrevistoProximoMes = transacoes
                                .Where(t => t.DataEfetivacao <= fimProximoMes)
                                .Sum(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);

        ViewBag.AnoAtual = anoAtual;
        ViewBag.MesAtual = mesAtual;

        return View(transacoes);
    }
}