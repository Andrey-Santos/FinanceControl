using FinanceControl.Core.Application.DTOs.Banco;
using FinanceControl.Core.Application.UseCases.Banco;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class BancoController : BaseController<Banco, BancoCreateDto, BancoResponseDto, BancoUpdateDto, BancoUseCase>
{
    public BancoController(BancoUseCase useCase) : base(useCase)
    {
    }
}