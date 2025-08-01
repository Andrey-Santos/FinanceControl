using FinanceControl.Core.Application.DTOs.TipoTransacao;
using FinanceControl.Core.Application.UseCases.TipoTransacao;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Financecontrol.WebApi.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class TipoTransacaoController : BaseController<TipoTransacao, TipoTransacaoCreateDto, TipoTransacaoResponseDto, TipoTransacaoUpdateDto, TipoTransacaoUseCase>
{
    public TipoTransacaoController(TipoTransacaoUseCase useCase) : base(useCase)
    {
    }
}