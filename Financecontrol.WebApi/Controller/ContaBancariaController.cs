using FinanceControl.Core.Application.DTOs.ContaBancaria;
using FinanceControl.Core.Application.UseCases.ContaBancaria;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class ContaBancariaController : BaseController<ContaBancaria, CreateContaBancariaDto, ContaBancariaResponseDto, ContaBancariaUseCase>
{
    public ContaBancariaController(ContaBancariaUseCase useCase) : base(useCase)
    {
    }
}
