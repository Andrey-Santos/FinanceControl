using FinanceControl.Core.Domain.Enums;

namespace FinanceControl.Core.Application.DTOs.Transacao;

public class TransacaoFilterDto
{
    public string? Descricao { get; set; }
    public decimal? ValorMinimo { get; set; }
    public decimal? ValorMaximo { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public TipoTransacao? Tipo { get; set; }
    public TipoOperacao? TipoOperacao { get; set; }
    public long? ContaBancariaId { get; set; }
    public long? CategoriaId { get; set; }
}
