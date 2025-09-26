namespace FinanceControl.Core.Domain.Entities;

public class Fatura : EntityBase
{
    public short Mes { get; set; }
    public short Ano { get; set; }
    public long CartaoId { get; set; }
    public Cartao Cartao { get; set; } = null!;
    public long ContaPagarReceberId { get; set; }
    public ContaPagarReceber ContaPagarReceber { get; set; } = null!;
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}