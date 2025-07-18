namespace FinanceControl.Core.Domain.Entities;

public class Cartao : EntityBase
{
    public string Apelido { get; set; } = string.Empty;
    public long BancoId { get; set; }
    public DateTime DataFechamento { get; set; }
    public long Tipo { get; set; }
    public long? Limite { get; set; }

    public Banco Banco { get; set; } = null!;
    public ICollection<Fatura> Faturas { get; set; } = new List<Fatura>();
}
