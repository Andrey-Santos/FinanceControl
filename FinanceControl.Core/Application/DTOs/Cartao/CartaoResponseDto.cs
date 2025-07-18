namespace FinanceControl.Core.Application.DTOs.Cartao;

public class CartaoResponseDto : DtoResponse
{
    public string Apelido { get; set; } = string.Empty;
    public long BancoId { get; set; }
    public DateTime DataFechamento { get; set; }
    public long Tipo { get; set; }
    public long? Limite { get; set; }
}