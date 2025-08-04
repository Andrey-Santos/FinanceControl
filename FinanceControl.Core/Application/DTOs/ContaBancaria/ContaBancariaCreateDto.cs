namespace FinanceControl.Core.Application.DTOs.ContaBancaria;

public class ContaBancariaCreateDto
{
    public long UsuarioId { get; set; }
    public string Numero { get; set; } = string.Empty;
    public long BancoId { get; set; }
}