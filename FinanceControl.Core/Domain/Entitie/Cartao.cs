namespace FinanceControl.Core.Domain.Entities;

public class Cartao : EntityBase
{
    public string Apelido { get; set; } = string.Empty;
    public long ContaBancariaId  { get; set; }
    public DateTime DataFechamento { get; set; }
    public long Tipo { get; set; }
    public long? Limite { get; set; }

    public ContaBancaria ContaBancaria { get; set; } = null!;
    public ICollection<Fatura> Faturas { get; set; } = new List<Fatura>();
}
