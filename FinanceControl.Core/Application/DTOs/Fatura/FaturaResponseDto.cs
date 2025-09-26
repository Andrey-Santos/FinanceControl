namespace FinanceControl.Core.Application.DTOs.Fatura;

public class FaturaResponseDto : DtoResponse
{
    public long ContaPagarReceberId { get; set; }
    public long CartaoId { get; set; }
    public short Mes { get; set; }
    public short Ano { get; set; }
    public string CartaoApelido { get; set; } = null!;
    public decimal ValorTotal { get; set; }
}