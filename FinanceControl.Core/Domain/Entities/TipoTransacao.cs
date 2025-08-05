namespace FinanceControl.Core.Domain.Entities;

public class TipoTransacao : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
}
