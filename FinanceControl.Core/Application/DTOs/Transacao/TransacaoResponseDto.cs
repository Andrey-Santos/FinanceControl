namespace FinanceControl.Core.Application.DTOs.Transacao;

public class TransacaoResponseDto : DtoResponse
{
    public string Descricao { get; set; } = string.Empty;
    public double Valor { get; set; }
    public DateTime DataEfetivacao { get; set; }
    public long ContaBancariaId { get; set; }
    public long CategoriaId { get; set; }

    public string ContaBancariaNumero { get; set; } = string.Empty;
    public string CategoriaNome { get; set; } = string.Empty;
}