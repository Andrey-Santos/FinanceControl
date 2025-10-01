using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient; // troque para Npgsql ou MySql se usar outro banco
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace FinanceControl.WebApi.Controllers;

[JwtAuthorize]
public class QueryController : Controller
{
    private readonly IConfiguration _config;
    private readonly string? _connectionString;

    public QueryController(IConfiguration config)
    {
        _config = config;
        _connectionString = _config.GetConnectionString("DefaultConnection");
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
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandTimeout = 120; // 2 minutos
            cmd.CommandText = sqlQuery;

            int affected = await cmd.ExecuteNonQueryAsync();

            ViewBag.Message = $"Query executada com sucesso. Linhas afetadas: {affected}";
        }
        catch (Exception ex)
        {
            ViewBag.Error = "Erro ao executar: " + ex.Message;
        }

        ViewBag.Query = sqlQuery;
        return View("Index");
    }
}
