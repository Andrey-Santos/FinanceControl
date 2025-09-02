using FinanceControl.Core.Application.DTOs.Usuario;
using FinanceControl.Core.Application.UseCases.Usuario;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Financecontrol.WebApi.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : BaseController<Usuario, UsuarioCreateDto, UsuarioResponseDto, UsuarioResponseDto, UsuarioUseCase>
{
    public UsuarioController(UsuarioUseCase useCase) : base(useCase)
    {
    }
}