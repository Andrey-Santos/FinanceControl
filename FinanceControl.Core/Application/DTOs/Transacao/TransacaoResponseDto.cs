using FinanceControl.Core.Domain.Enums;

namespace FinanceControl.Core.Application.DTOs.Transacao;

public class TransacaoResponseDto : DtoResponse
{
    public string Descricao { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public DateTime DataEfetivacao { get; set; }
    public long ContaBancariaId { get; set; }
    public long CategoriaId { get; set; }
    public long? CartaoId { get; set; }
    public long? FaturaId { get; set; }
    public string ContaBancariaNumero { get; set; } = string.Empty;
    public string CategoriaNome { get; set; } = string.Empty;
    public TipoTransacao Tipo { get; set; }
    public TipoOperacao TipoOperacao { get; set; }
    public string? Observacao { get; set; }

    public TransacaoResponseDto(Domain.Entities.Transacao entity)
    {
        Id = entity.Id;
        Descricao = entity.Descricao;
        Valor = entity.Valor;
        DataEfetivacao = entity.DataEfetivacao;
        ContaBancariaId = entity.ContaBancariaId;
        CategoriaId = entity.CategoriaId;
        CartaoId = entity.CartaoId;
        FaturaId = entity.FaturaId;
        ContaBancariaNumero = entity.ContaBancaria?.Numero ?? string.Empty;
        CategoriaNome = entity.Categoria?.Nome ?? string.Empty;
        Tipo = entity.Tipo;
        TipoOperacao = entity.TipoOperacao;
        Observacao = entity.Observacao;
    }
}
