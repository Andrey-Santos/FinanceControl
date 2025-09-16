using Microsoft.AspNetCore.Mvc;
using FinanceControl.Core.Application.UseCases.Fatura;
using FinanceControl.Core.Application.DTOs.Fatura;

namespace Financecontrol.WebApi.Controllers.Web;

public class FaturaController : Controller
{
    private readonly FaturaUseCase _useCase;

    public FaturaController(FaturaUseCase useCase)
    {
        _useCase = useCase;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View(await _useCase.GetAllAsync());
    }
}
