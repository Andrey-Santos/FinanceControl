using DocumentFormat.OpenXml.EMMA;
using FinanceControl.Core.Application.DTOs.Login;
using FinanceControl.Core.Application.DTOs.Usuario;
using FinanceControl.Core.Application.UseCases.Usuario;
using FinanceControl.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtTokenService _jwtService;
    private readonly UsuarioUseCase _usuarioUseCase;

    public AuthController(IUsuarioRepository usuarioRepository, IJwtTokenService jwtService, UsuarioUseCase usuarioUseCase)
    {
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
        _usuarioUseCase = usuarioUseCase;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(dto.Email);
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            return Unauthorized("Usuário ou senha inválidos");

        var token = _jwtService.GenerateToken(usuario);

        return Ok(new LoginResponseDto
        {
            Token = token,
            Nome = usuario.Nome,
            Email = usuario.Email
        });
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] LoginCreateDto dto)
    {
         UsuarioCreateDto model = new UsuarioCreateDto();
        model.Email = dto.Email;
        model.Nome = dto.Nome;
        model.Senha = dto.Senha;

        var resultado = await _usuarioUseCase.AddAsync(model);
        if (resultado == 0)
            return BadRequest(resultado);

        return Ok("Usuário criado com sucesso.");
    }
}