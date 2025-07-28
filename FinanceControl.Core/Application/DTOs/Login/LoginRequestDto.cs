namespace FinanceControl.Core.Application.DTOs.Login;

public class LoginRequestDto
{
    public string Nome { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}