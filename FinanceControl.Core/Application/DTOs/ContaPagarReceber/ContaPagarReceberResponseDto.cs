using FinanceControl.Core.Domain.Enums;

namespace FinanceControl.Core.Application.DTOs.ContaPagarReceber;

public class ContaPagarReceberResponseDto
{
    public long Id { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataVencimento { get; set; }
    public DateTime? DataPagamento { get; set; }
    public StatusContaPagarReceber Status { get; set; }
    public TipoTransacao Tipo { get; set; }
    public long? CategoriaId { get; set; }
    public long ContaBancariaId { get; set; }
    public string ContaBancariaNumero { get; set; } = string.Empty;
    public long? TransacaoId { get; set; }
}