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
    public TransacaoCreateDto()
    {
    }
    public TransacaoCreateDto(Domain.Entities.Transacao model)
    {
        Descricao = model.Descricao;
        Valor = model.Valor;
        DataEfetivacao = model.DataEfetivacao;
        ContaBancariaId = model.ContaBancariaId;
        CategoriaId = model.CategoriaId;
        CartaoId = model.CartaoId;
        FaturaId = model.FaturaId;
        Tipo = model.Tipo;
        TipoOperacao = model.TipoOperacao;
        Observacao = model.Observacao;
    }
    public TransacaoCreateDto(TransacaoResponseDto model)
    {
        Descricao = model.Descricao;
        Valor = model.Valor;
        DataEfetivacao = model.DataEfetivacao;
        ContaBancariaId = model.ContaBancariaId;
        CategoriaId = model.CategoriaId;
        CartaoId = model.CartaoId;
        FaturaId = model.FaturaId;
        Tipo = model.Tipo;
        TipoOperacao = model.TipoOperacao;
        Observacao = model.Observacao;
    }
}