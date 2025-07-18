namespace FinanceControl.Core.Application.DTOs.Fatura;

public class CreateFaturaDto
{
    public long CartaoId { get; set; }
    public short Mes { get; set; }
    public short Ano { get; set; }
}
