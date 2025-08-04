namespace FinanceControl.Core.Application.DTOs.ContaBancaria;

public class ContaBancariaResponseDto : DtoResponse
{
    public string Numero { get; set; } = string.Empty;

    public long BancoId { get; set; }
    public string BancoNome { get; set; } = string.Empty;
    public long UsuarioId { get; set; }

}
