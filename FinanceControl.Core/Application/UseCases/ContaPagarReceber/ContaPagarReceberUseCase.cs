using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.ContaPagarReceber;
using Microsoft.EntityFrameworkCore;
using FinanceControl.Core.Domain.Enums;
using FinanceControl.Core.Application.DTOs.Transacao;
using FinanceControl.Core.Application.UseCases.Transacao;

namespace FinanceControl.Core.Application.UseCases.ContaPagarReceber;

public class ContaPagarReceberUseCase : BaseUseCase, IBaseUseCase<Domain.Entities.ContaPagarReceber, ContaPagarReceberCreateDto, ContaPagarReceberResponseDto, ContaPagarReceberUpdateDto>
{
    private readonly IContaPagarReceberRepository _repository;
    private readonly IContaBancariaRepository _contaBancariaRepository;
    private readonly TransacaoUseCase _transacaoUseCase;
    private readonly IUnitOfWork _unitOfWork;

    public ContaPagarReceberUseCase(
        IContaPagarReceberRepository repository,
        IContaBancariaRepository contaBancariaRepository,
        TransacaoUseCase transacaoUseCase,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _contaBancariaRepository = contaBancariaRepository;
        _transacaoUseCase = transacaoUseCase;
    }

    public async Task<IEnumerable<ContaPagarReceberResponseDto>> GetAllAsync()
    {
        var contas = await _repository
            .GetAll()
            .Include(t => t.ContaBancaria)
            .ToListAsync();

        return contas.Select(u => new ContaPagarReceberResponseDto
        {
            Id = u.Id,
            Descricao = u.Descricao,
            Valor = u.Valor,
            DataPagamento = u.DataPagamento,
            DataVencimento = u.DataVencimento,
            ContaBancariaId = u.ContaBancariaId,
            ContaBancariaNumero = u.ContaBancaria.Numero,
            Tipo = u.Tipo,
            Status = u.Status,
            TransacaoId = u.TransacaoId
        });
    }

    public async Task<ContaPagarReceberResponseDto?> GetByIdAsync(long id)
    {
        var conta = await _repository.GetByIdAsync(id);

        return conta is null ? null : new ContaPagarReceberResponseDto
        {
            Id = conta.Id,
            Descricao = conta.Descricao,
            Valor = conta.Valor,
            DataPagamento = conta.DataPagamento,
            DataVencimento = conta.DataVencimento,
            ContaBancariaId = conta.ContaBancariaId,
            Tipo = conta.Tipo,
            Status = conta.Status,
            TransacaoId = conta.TransacaoId
        };
    }

    public async Task<long> AddAsync(ContaPagarReceberCreateDto dto)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, dto.ContaBancariaId, "Conta bancária");

        if (!dto.DataPagamento.HasValue)
            dto.Status = StatusContaPagarReceber.Aberta;
        else
            dto.Status = StatusContaPagarReceber.Paga;

        var conta = new Domain.Entities.ContaPagarReceber
        {
            Descricao = dto.Descricao,
            DataPagamento = dto.DataPagamento,
            Valor = Math.Abs(dto.Valor),
            ContaBancariaId = dto.ContaBancariaId,
            DataVencimento = dto.DataVencimento,
            Tipo = dto.Tipo,
            Status = dto.Status,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        await _repository.AddAsync(conta);
        await _unitOfWork.CommitAsync();

        return conta.Id;
    }

    public async Task UpdateAsync(ContaPagarReceberUpdateDto dto)
    {
        var conta = await _repository.GetByIdAsync(dto.Id);
        if (conta == null)
            throw new ArgumentException("Conta não encontrada.");

        conta.Descricao = dto.Descricao;
        conta.DataPagamento = dto.DataPagamento;
        conta.Valor = Math.Abs(dto.Valor);
        conta.ContaBancariaId = dto.ContaBancariaId;
        conta.Tipo = dto.Tipo;
        conta.DataVencimento = dto.DataVencimento;
        conta.Status = dto.DataPagamento.HasValue ? StatusContaPagarReceber.Paga : StatusContaPagarReceber.Aberta;
        conta.DataAlteracao = DateTime.UtcNow;

        await _repository.UpdateAsync(conta);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(Domain.Entities.ContaPagarReceber model)
    {
        var conta = await _repository.GetByIdAsync(model.Id);
        if (conta == null)
            throw new ArgumentException("Conta não encontrada.");

        conta.Descricao = model.Descricao;
        conta.DataPagamento = model.DataPagamento;
        conta.Valor = Math.Abs(model.Valor);
        conta.ContaBancariaId = model.ContaBancariaId;
        conta.Tipo = model.Tipo;
        conta.DataVencimento = model.DataVencimento;
        conta.Status = model.DataPagamento.HasValue ? StatusContaPagarReceber.Paga : StatusContaPagarReceber.Aberta;
        conta.DataAlteracao = DateTime.UtcNow;

        await _repository.UpdateAsync(conta);
        await _unitOfWork.CommitAsync();
    }

    public async Task PagarAsync(long id, DateTime dataPagamento)
    {
        var conta = await _repository.GetByIdAsync(id);
        if (conta == null)
            throw new ArgumentException("Conta não encontrada.");

        conta.DataPagamento = dataPagamento;
        conta.Status = StatusContaPagarReceber.Paga;
        conta.DataAlteracao = DateTime.UtcNow;

        if (conta.TransacaoId == null)
        {
            var transacao = new TransacaoCreateDto
            {
                Descricao = conta.Descricao,
                Valor = conta.Valor,
                DataEfetivacao = dataPagamento,
                ContaBancariaId = conta.ContaBancariaId,
                Tipo = conta.Tipo,
                TipoOperacao = TipoOperacao.Debito
            };

            conta.TransacaoId = await _transacaoUseCase.AddAsync(transacao);
        }

        await _repository.UpdateAsync(conta);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }

    public async Task<decimal> GetSaldoPrevistoProximoMes(int mesAtual, int anoAtual, long usuarioId)
    {
        var inicioMesAtual = new DateTime(anoAtual, mesAtual, 1);
        var fimMesAtual = inicioMesAtual.AddMonths(1).AddDays(-1);
        var fimProximoMes = inicioMesAtual.AddMonths(2).AddDays(-1);

        var baseQuery = _repository
            .GetAll()
            .AsNoTracking()
            .Select(t => new
            {
                t.Valor,
                t.Tipo,
                t.ContaBancaria.UsuarioId,
                t.DataPagamento,
                t.DataVencimento
            })
            .Where(t =>
                t.DataVencimento >= fimMesAtual &&
                t.DataVencimento <= fimProximoMes &&
                t.UsuarioId == usuarioId &&
                t.DataPagamento == null);

        return await baseQuery.SumAsync(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);
    }
}
