using FinanceControl.Core.Application.DTOs.ContaPagarReceber;
using FinanceControl.Core.Application.UseCases.ContaPagarReceber;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class ContaPagarReceberController : BaseController<ContaPagarReceber, ContaPagarReceberCreateDto, ContaPagarReceberResponseDto, ContaPagarReceberUpdateDto, ContaPagarReceberUseCase>
{
    public ContaPagarReceberController(ContaPagarReceberUseCase useCase) : base(useCase) { }
}
