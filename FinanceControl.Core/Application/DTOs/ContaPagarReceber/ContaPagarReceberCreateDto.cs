using FinanceControl.Core.Domain.Enums;

namespace FinanceControl.Core.Application.DTOs.ContaPagarReceber;

public class ContaPagarReceberCreateDto
{
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public TipoTransacao Tipo { get; set; }
    public long ContaBancariaId { get; set; }
    public long CategoriaId { get; set; }
    public StatusContaPagarReceber Status { get; set; }
}