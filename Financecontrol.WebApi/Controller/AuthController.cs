using FinanceControl.Core.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IJwtTokenService _jwtService;

    public AuthController(IUsuarioRepository usuarioRepository, IJwtTokenService jwtService)
    {
        _usuarioRepository = usuarioRepository;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto dto)
    {
        var usuario = await _usuarioRepository.GetByUsernameAsync(dto.Username);
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.SenhaHash))
            return Unauthorized("Usuário ou senha inválidos");

        var token = _jwtService.GenerateToken(usuario);

        return Ok(new LoginResponseDto
        {
            Token = token,
            Username = usuario.Nome
        });
    }
}
