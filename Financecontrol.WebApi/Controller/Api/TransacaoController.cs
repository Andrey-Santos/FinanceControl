using FinanceControl.Core.Application.DTOs.Transacao;
using FinanceControl.Core.Application.UseCases.Transacao;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Financecontrol.WebApi.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TransacaoController : BaseController<Transacao, TransacaoCreateDto, TransacaoResponseDto, TransacaoUpdateDto, TransacaoUseCase>
{
    public TransacaoController(TransacaoUseCase useCase) : base(useCase)
    {
    }

    [HttpGet("PorMes")]
    public async Task<IActionResult> GetPorMes(int mes, int ano)
    {
        var result = await _useCase.GetByFiltroAsync(new DateTime(ano, mes, 1), new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes)));
        return Ok(result);
    }

}