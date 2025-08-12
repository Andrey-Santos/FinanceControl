namespace FinanceControl.Core.Domain.Entities;

public class Transacao : EntityBase
{
    public string Descricao { get; set; } = string.Empty;
    public double Valor { get; set; }
    public DateTime DataEfetivacao { get; set; }
    public long ContaBancariaId { get; set; }
    public long CategoriaId { get; set; }

    public ContaBancaria ContaBancaria { get; set; } = null!;
    public CategoriaTransacao Categoria { get; set; } = null!;
}
