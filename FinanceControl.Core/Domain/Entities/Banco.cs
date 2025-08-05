namespace FinanceControl.Core.Domain.Entities;

public class Banco : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public ICollection<ContaBancaria> ContasBancarias { get; set; } = new List<ContaBancaria>();
    public ICollection<Cartao> Cartoes { get; set; } = new List<Cartao>();
}