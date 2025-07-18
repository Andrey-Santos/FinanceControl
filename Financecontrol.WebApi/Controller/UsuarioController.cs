using FinanceControl.Core.Application.DTOs.Usuario;
using FinanceControl.Core.Application.UseCases.Usuario;
using FinanceControl.Core.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Financecontrol.WebApi.Controller;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : BaseController<Usuario, CreateUsuarioDto, UsuarioResponseDto, UsuarioUseCase>
{
    public UsuarioController(UsuarioUseCase useCase) : base(useCase)
    {
    }
}