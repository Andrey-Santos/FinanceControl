namespace FinanceControl.Core.Domain.Entities;

public class Fatura : EntityBase
{
    public long CartaoId { get; set; }
    public short Mes { get; set; }
    public short Ano { get; set; }
    public Cartao Cartao { get; set; } = null!;
}