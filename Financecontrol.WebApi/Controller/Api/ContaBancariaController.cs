using FinanceControl.Core.Application.DTOs.ContaBancaria;
using FinanceControl.Core.Application.UseCases.ContaBancaria;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class ContaBancariaController : BaseController<ContaBancaria, ContaBancariaCreateDto, ContaBancariaResponseDto, ContaBancariaResponseDto, ContaBancariaUseCase>
{
    public ContaBancariaController(ContaBancariaUseCase useCase) : base(useCase)
    {
    }
}
