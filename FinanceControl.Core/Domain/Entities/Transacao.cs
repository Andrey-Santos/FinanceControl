using FinanceControl.Core.Domain.Enums;

namespace FinanceControl.Core.Domain.Entities;

public class Transacao : EntityBase
{
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataEfetivacao { get; set; }
    public long ContaBancariaId { get; set; }
    public long CategoriaId { get; set; }
    public TipoTransacao Tipo { get; set; }
    public string? Observacao { get; set; }
    public TipoOperacao TipoOperacao { get; set; }
    public long? CartaoId { get; set; }
    public long? FaturaId { get; set; }
    public ContaBancaria ContaBancaria { get; set; } = null!;
    public Cartao? Cartao { get; set; }
    public CategoriaTransacao Categoria { get; set; } = null!;
    public Fatura? Fatura { get; set; } = null;
}
