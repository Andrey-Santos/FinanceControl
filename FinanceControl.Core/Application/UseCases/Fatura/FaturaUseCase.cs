using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.Fatura;
using Microsoft.EntityFrameworkCore;
using FinanceControl.Core.Application.DTOs.Transacao;
using FinanceControl.Core.Application.UseCases.ContaPagarReceber;
using FinanceControl.Core.Application.DTOs.ContaPagarReceber;
using FinanceControl.Core.Domain.Enums;

namespace FinanceControl.Core.Application.UseCases.Fatura;

public class FaturaUseCase : BaseUseCase, IBaseUseCase<Domain.Entities.Fatura, CreateFaturaDto, FaturaResponseDto, FaturaResponseDto>
{
    private readonly IFaturaRepository _repository;
    private readonly ICartaoRepository _cartaoRepository;
    private readonly IContaPagarReceberRepository _contaPagarReceberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FaturaUseCase(IFaturaRepository repository,
                        ICartaoRepository cartaoRepository,
                        IContaPagarReceberRepository contaPagarReceberRepository,
                        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _cartaoRepository = cartaoRepository;
        _contaPagarReceberRepository = contaPagarReceberRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IEnumerable<FaturaResponseDto>> GetAllAsync()
    {
        var faturas = await _repository
            .GetAll()
            .Include(f => f.Cartao)
            .Include(f => f.ContaPagarReceber)
            .ToListAsync();

        return faturas.Select(u => new FaturaResponseDto
        {
            Id = u.Id,
            Mes = u.Mes,
            Ano = u.Ano,
            CartaoId = u.CartaoId,
            CartaoApelido = u.Cartao.Apelido,
            ValorTotal = u.ContaPagarReceber?.Valor ?? 0,
        });
    }

    public async Task<FaturaResponseDto?> GetByIdAsync(long id)
    {
        var Fatura = await _repository.GetByIdAsync(id);
        return Fatura is null ? null : new FaturaResponseDto
        {
            Id = Fatura.Id,
            Mes = Fatura.Mes,
            Ano = Fatura.Ano,
            CartaoId = Fatura.CartaoId
        };
    }
    public async Task<long?> GetFaturaFromTransacao(TransacaoCreateDto transacaoCreateDto)
    {
        long cartaoId = (long)(transacaoCreateDto.CartaoId ?? 0);
        if (cartaoId == 0)
            return 0;

        (short mes, short ano) = await GetMesAnoFromCartao(transacaoCreateDto.DataEfetivacao, cartaoId);
        var Fatura = await _repository.GetByCartaoEFaturaAsync(cartaoId, mes, ano);

        return Fatura?.Id ?? 0;        
    }
    public async Task<long> CreateFaturaFromTransacaoAsync(TransacaoCreateDto dto, long contaPagarReceberId)
    {
        long cartaoId = (long)(dto.CartaoId ?? 0);

        await ValidarEntidadeExistenteAsync(_cartaoRepository, cartaoId, "Cartão");

        (short mes, short ano) = await GetMesAnoFromCartao(dto.DataEfetivacao, cartaoId);

        var novaFatura = new CreateFaturaDto
        {
            CartaoId = cartaoId,
            Mes = mes,
            Ano = ano,
            ContaPagarReceberId = contaPagarReceberId
        };

        long faturaId = await AddAsync(novaFatura);
        return faturaId;
    }
    public void AtualizarValorContaPagarReceber(long faturaId)
    {
        var fatura = _repository.GetByIdAsync(faturaId).Result;
        if (fatura == null || fatura.ContaPagarReceberId == 0)
            return;

        decimal valorTotal = _repository
            .GetAll()
            .Include(t => t.Transacoes)
            .Where(f => f.Id == faturaId)
            .SelectMany(f => f.Transacoes)
            .Sum(t => t.Valor);

        var contaPagarReceber = _contaPagarReceberRepository.GetByIdAsync((long)fatura.ContaPagarReceberId!).Result;
        if (contaPagarReceber == null)
            return;

        contaPagarReceber.Valor = valorTotal;
        _contaPagarReceberRepository.UpdateAsync(contaPagarReceber).Wait();
        _unitOfWork.CommitAsync().Wait();
    }
    
    public async Task<(short, short)> GetMesAnoFromCartao(DateTime dataEfetivacao, long cartaoId)
    {
        (short transacaoMes, short transacaoAno) = ((short)dataEfetivacao.Month, (short)dataEfetivacao.Year);

        var cartao = await _cartaoRepository.GetByIdAsync(cartaoId);

        if (dataEfetivacao.Day > cartao?.DiaFechamento)
        {
            transacaoMes = (short)(dataEfetivacao.Month == 12 ? 1 : dataEfetivacao.Month + 1);
            transacaoAno = (short)(dataEfetivacao.Month == 12 ? dataEfetivacao.Year + 1 : dataEfetivacao.Year);
        }

        return (transacaoMes, transacaoAno);
    }
    public async Task<long> AddAsync(CreateFaturaDto dto)
    {
        await ValidarEntidadeExistenteAsync(_cartaoRepository, dto.CartaoId, "Cartão");

        var Fatura = new Domain.Entities.Fatura
        {
            CartaoId = dto.CartaoId,
            Mes = dto.Mes,
            Ano = dto.Ano,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow,
            ContaPagarReceberId = dto.ContaPagarReceberId
        };

        await _repository.AddAsync(Fatura);
        await _unitOfWork.CommitAsync();

        return Fatura.Id;
    }
    public async Task UpdateAsync(FaturaResponseDto dto)
    {
        await ValidarEntidadeExistenteAsync(_cartaoRepository, dto.CartaoId, "Cartão");

        var Fatura = await _repository.GetByIdAsync(dto.Id);
        if (Fatura == null)
            return;

        Fatura.DataAlteracao = DateTime.UtcNow;
        Fatura.CartaoId = dto.CartaoId;
        Fatura.Mes = dto.Mes;
        Fatura.Ano = dto.Ano;
        Fatura.DataAlteracao = DateTime.UtcNow;

        await _repository.UpdateAsync(Fatura);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}
