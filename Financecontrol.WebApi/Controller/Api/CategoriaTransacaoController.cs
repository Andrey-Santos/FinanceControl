using FinanceControl.Core.Application.DTOs.CategoriaTransacao;
using FinanceControl.Core.Application.UseCases.CategoriaTransacao;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Financecontrol.WebApi.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class CategoriaTransacaoController : BaseController<CategoriaTransacao, CategoriaTransacaoCreateDto, CategoriaTransacaoResponseDto, CategoriaTransacaoUpdateDto, CategoriaTransacaoUseCase>
{
    public CategoriaTransacaoController(CategoriaTransacaoUseCase useCase) : base(useCase)
    {
    }
}