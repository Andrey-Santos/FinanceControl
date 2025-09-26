using FinanceControl.Core.Application.DTOs.Transacao;
using FinanceControl.Core.Application.DTOs.Fatura;
using FinanceControl.Core.Application.DTOs.ContaPagarReceber;
using FinanceControl.Core.Application.UseCases.Transacao;
using FinanceControl.Core.Application.UseCases.Fatura;
using FinanceControl.Core.Application.UseCases.ContaPagarReceber;
using FinanceControl.Core.Domain.Enums;
using FinanceControl.Core.Domain.Entities;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.UseCases.ContaBancaria;
using FinanceControl.Core.Application.UseCases.Cartao;

namespace FinanceControl.Core.Application.Workflows;

public class FinanceWorkflowService
{
    private readonly TransacaoUseCase _transacaoUseCase;
    private readonly ITransacaoRepository _transacaoRepository;
    private readonly FaturaUseCase _faturaUseCase;
    private readonly CartaoUseCase _cartaoUsecase;
    private readonly ContaPagarReceberUseCase _contaPagarReceberUseCase;

    public FinanceWorkflowService(
        TransacaoUseCase transacaoUseCase,
        FaturaUseCase faturaUseCase,
        ContaPagarReceberUseCase contaPagarReceberUseCase,
        CartaoUseCase cartaoUseCase,  
        ITransacaoRepository transacaoRepository)
    {
        _transacaoUseCase = transacaoUseCase;
        _faturaUseCase = faturaUseCase;
        _contaPagarReceberUseCase = contaPagarReceberUseCase;
        _cartaoUsecase = cartaoUseCase;
        _transacaoRepository = transacaoRepository;
    }

    // -------------------------------
    // TRANSACOES
    // -------------------------------

    public async Task<long> RegistrarTransacaoAsync(TransacaoCreateDto dto)
    {
        if (dto.TipoOperacao == TipoOperacao.Credito)
        {
            dto.FaturaId = await _faturaUseCase.GetFaturaFromTransacao(dto);
            if (dto.FaturaId == 0)
            {
                long cartaoId = dto.CartaoId ?? 0;
                var cartao = await _cartaoUsecase.GetByIdAsync(cartaoId);
                (short mes, short ano) = await _faturaUseCase.GetMesAnoFromCartao(dto.DataEfetivacao, cartaoId);
                
                var contaId = await _contaPagarReceberUseCase.AddAsync(new ContaPagarReceberCreateDto
                {
                    Descricao = $"Fatura  {cartao?.Apelido} - {mes}/{ano}",
                    ContaBancariaId = dto.ContaBancariaId,
                    DataVencimento = new DateTime(ano, mes, 10),
                    Tipo = TipoTransacao.Despesa
                });

                dto.FaturaId = await _faturaUseCase.CreateFaturaFromTransacaoAsync(dto, contaId);
            }
        }

        long transacaoId = await _transacaoUseCase.AddAsync(dto);
        if (dto.FaturaId > 0)
            _faturaUseCase.AtualizarValorContaPagarReceber((long)dto.FaturaId);

        return transacaoId;
    }

    public async Task<IEnumerable<TransacaoResponseDto>> GetAllTransacoesAsync()
        => await _transacaoUseCase.GetAllAsync();

    public async Task<IEnumerable<TransacaoResponseDto>> GetFilteredTransacoesAsync(TransacaoFilterDto filtro)
        => await _transacaoUseCase.GetFilteredAsync(filtro);

    public async Task<TransacaoResponseDto?> GetTransacaoByIdAsync(long id)
        => await _transacaoUseCase.GetByIdAsync(id);

    public async Task UpdateTransacaoAsync(TransacaoUpdateDto dto)
        => await _transacaoUseCase.UpdateAsync(dto);

    public async Task DeleteTransacaoAsync(long id)
    {
        var transacao = await _transacaoRepository.GetByIdAsync(id);
        if (transacao?.Fatura != null)
        {
            long faturaId = transacao.FaturaId ?? 0;
            await _transacaoUseCase.DeleteAsync(id);
            _faturaUseCase.AtualizarValorContaPagarReceber(faturaId);
            return;
        }
        else
            await _transacaoUseCase.DeleteAsync(id);
    }

    // -------------------------------
    // FATURAS
    // -------------------------------

    public async Task<long> FecharFaturaAsync(long faturaId)
    {
        var fatura = await _faturaUseCase.GetByIdAsync(faturaId);
        if (fatura == null)
            throw new ArgumentException("Fatura não encontrada.");

        // TODO: Somar valor das transações dessa fatura
        decimal valorTotal = 0;

        var contaId = await _contaPagarReceberUseCase.AddAsync(new ContaPagarReceberCreateDto
        {
            Descricao = $"Fatura {fatura.Mes}/{fatura.Ano}",
            Valor = valorTotal,
            ContaBancariaId = 1, // TODO: buscar do cartão
            DataVencimento = new DateTime(fatura.Ano, fatura.Mes, 10),
            Tipo = TipoTransacao.Despesa,
            Status = StatusContaPagarReceber.Aberta
        });

        return contaId;
    }

    // -------------------------------
    // CONTAS A PAGAR/RECEBER
    // -------------------------------

    public async Task QuitarContaAsync(long contaId, DateTime dataPagamento)
        => await _contaPagarReceberUseCase.PagarAsync(contaId, dataPagamento);
}
