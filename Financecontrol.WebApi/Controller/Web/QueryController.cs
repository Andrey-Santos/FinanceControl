using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace FinanceControl.WebApi.Controllers;

[JwtAuthorize]
public class QueryController : Controller
{
    private readonly string? _connectionString;

    public QueryController(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Execute(string sqlQuery)
    {
        if (string.IsNullOrWhiteSpace(sqlQuery))
        {
            ViewBag.Error = "Query vazia.";
            return View("Index");
        }

        try
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync();

            await using var cmd = new NpgsqlCommand(sqlQuery, conn);
            int affected = await cmd.ExecuteNonQueryAsync();

            ViewBag.Message = $"Executado com sucesso. Linhas afetadas: {affected}";
        }
        catch (Exception ex)
        {
            ViewBag.Error = "Erro ao executar: " + ex.Message;
        }

        ViewBag.Query = sqlQuery;
        return View("Index");
    }
}
