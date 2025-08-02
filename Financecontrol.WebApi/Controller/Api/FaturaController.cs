using FinanceControl.Core.Application.DTOs.Fatura;
using FinanceControl.Core.Application.UseCases.Fatura;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class FaturaController : BaseController<Fatura, CreateFaturaDto, FaturaResponseDto, FaturaResponseDto, FaturaUseCase>
{
    public FaturaController(FaturaUseCase useCase) : base(useCase)
    {
    }
}
