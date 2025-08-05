using FinanceControl.Core.Domain.Enums;

namespace FinanceControl.Core.Application.DTOs.Cartao;

public class CartaoCreateDto
{
    public string Apelido { get; set; } = string.Empty;
    public long ContaBancariaId { get; set; }
    public int DiaFechamento { get; set; }
    public TipoCartao Tipo { get; set; }
    public decimal Limite { get; set; }
}
