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
            CategoriaId = conta.CategoriaId,
            Tipo = conta.Tipo,
            Status = conta.Status,
            TransacaoId = conta.TransacaoId
        };
    }

    public async Task<long> AddAsync(ContaPagarReceberCreateDto dto)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, dto.ContaBancariaId, "Conta bancária");

        var conta = new Domain.Entities.ContaPagarReceber
        {
            Descricao = dto.Descricao,
            DataPagamento = dto.DataPagamento,
            Valor = Math.Abs(dto.Valor),
            ContaBancariaId = dto.ContaBancariaId,
            DataVencimento = dto.DataVencimento,
            Tipo = dto.Tipo,
            Status = dto.DataPagamento.HasValue ? StatusContaPagarReceber.Paga : StatusContaPagarReceber.Aberta,
            CategoriaId = dto.CategoriaId,
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
        conta.CategoriaId = dto.CategoriaId;
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
}
