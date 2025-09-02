using FinanceControl.Core.Application.DTOs.Login;
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
        var usuario = await _usuarioRepository.GetByUsernameAsync(dto.Nome);
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
            return Unauthorized("Usuário ou senha inválidos");

        var token = _jwtService.GenerateToken(usuario);

        return Ok(new LoginResponseDto
        {
            Token = token,
            Nome = usuario.Nome
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] LoginCreateDto dto)
    {
        var usuarioExistente = await _usuarioRepository.GetByUsernameAsync(dto.Nome);
        if (usuarioExistente != null)
            return BadRequest("Nome de usuário já cadastrado.");

        // Criptografa a senha
        var senhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha);

        // Cria entidade de usuário (ajuste conforme sua entidade)
        var novoUsuario = new FinanceControl.Core.Domain.Entities.Usuario
        {
            Nome = dto.Nome,
            //Email = dto.Email,
            SenhaHash = senhaHash
        };

        await _usuarioRepository.AddAsync(novoUsuario);

        return Ok("Usuário criado com sucesso.");
    }
}