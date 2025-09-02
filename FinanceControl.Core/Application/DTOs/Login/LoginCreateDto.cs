namespace FinanceControl.Core.Application.DTOs.Login;

public class LoginCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}