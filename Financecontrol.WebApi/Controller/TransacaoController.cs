using FinanceControl.Core.Application.DTOs.Transacao;
using FinanceControl.Core.Application.UseCases.Transacao;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Financecontrol.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransacaoController : BaseController<Transacao, CreateTransacaoDto, TransacaoResponseDto, TransacaoUseCase>
{
    public TransacaoController(TransacaoUseCase useCase) : base(useCase)
    {
    }
}