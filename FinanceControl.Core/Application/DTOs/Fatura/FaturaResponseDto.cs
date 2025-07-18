namespace FinanceControl.Core.Application.DTOs.Fatura;

public class FaturaResponseDto : DtoResponse
{
    public long CartaoId { get; set; }
    public short Mes { get; set; }
    public short Ano { get; set; }
}