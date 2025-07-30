using FinanceControl.Core.Application.DTOs.Cartao;
using FinanceControl.Core.Application.UseCases.Cartao;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FinanceControl.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartaoController : BaseController<Cartao, CreateCartaoDto, CartaoResponseDto, CartaoUseCase>
{
    public CartaoController(CartaoUseCase useCase) : base(useCase)
    {
    }
}
