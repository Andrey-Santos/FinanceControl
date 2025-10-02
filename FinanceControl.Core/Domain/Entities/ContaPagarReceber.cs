using FinanceControl.Core.Domain.Enums;

namespace FinanceControl.Core.Domain.Entities;

public class ContaPagarReceber : EntityBase
{
    public string Descricao { get; set; } = string.Empty;

    public decimal Valor { get; set; }

    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusContaPagarReceber Status { get; set; }
    public TipoTransacao Tipo { get; set; }
    public long? CategoriaId { get; set; }
    public CategoriaTransacao Categoria { get; set; } = null!;
    public long ContaBancariaId { get; set; }
    public ContaBancaria ContaBancaria { get; set; } = null!;
    public long? TransacaoId { get; set; }
    public Transacao? Transacao { get; set; }
}
