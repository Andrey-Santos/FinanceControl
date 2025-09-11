using FinanceControl.Core.Domain.Enums;

namespace FinanceControl.Core.Application.DTOs.Transacao;

public class TransacaoCreateDto
{
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataEfetivacao { get; set; }
    public long CategoriaId { get; set; }
    public long ContaBancariaId { get; set; }
    public long? CartaoId { get; set; }
    public long? FaturaId { get; set; }
    public TipoTransacao Tipo { get; set; }
    public TipoOperacao TipoOperacao { get; set; }
    public string? Observacao { get; set; }
}
