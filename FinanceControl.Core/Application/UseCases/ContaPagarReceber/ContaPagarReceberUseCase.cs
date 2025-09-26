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

    public ContaPagarReceberUseCase(IContaPagarReceberRepository repository, IContaBancariaRepository contaBancariaRepository, TransacaoUseCase transacaoUseCase, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _contaBancariaRepository = contaBancariaRepository;
        _transacaoUseCase = transacaoUseCase;
    }

    public async Task<IEnumerable<ContaPagarReceberResponseDto>> GetAllAsync()
    {
        var ContaPagarRecebers = await _repository
                                .GetAll()
                                .Include(t => t.ContaBancaria)
                                .ToListAsync();

        return ContaPagarRecebers.Select(u => new ContaPagarReceberResponseDto
        {
            Id = u.Id,
            Descricao = u.Descricao,
            Valor = u.Valor,
            DataPagamento = u.DataPagamento,
            DataVencimento = u.DataVencimento,
            ContaBancariaNumero = u.ContaBancaria.Numero,
            Tipo = u.Tipo,
            Status = u.Status,
        });
    }

    public async Task<ContaPagarReceberResponseDto?> GetByIdAsync(long id)
    {
        var ContaPagarReceber = await _repository.GetByIdAsync(id);

        return ContaPagarReceber is null ? null : new ContaPagarReceberResponseDto
        {
            Id = ContaPagarReceber.Id,
            Descricao = ContaPagarReceber.Descricao,
            Valor = ContaPagarReceber.Valor,
            DataPagamento = ContaPagarReceber.DataPagamento,
            DataVencimento = ContaPagarReceber.DataVencimento,
            ContaBancariaId = ContaPagarReceber.ContaBancariaId,
            Tipo = ContaPagarReceber.Tipo
        };
    }

    public async Task<long> AddAsync(ContaPagarReceberCreateDto dto)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, dto.ContaBancariaId, "Conta bancária");

        //if (dto.Valor <= 0)
          //  throw new ArgumentException("O valor deve ser maior que zero.");

        if (!dto.DataPagamento.HasValue)
            dto.Status = StatusContaPagarReceber.Aberta;

        var ContaPagarReceber = new Domain.Entities.ContaPagarReceber
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

        await _repository.AddAsync(ContaPagarReceber);
        await _unitOfWork.CommitAsync();

        return ContaPagarReceber.Id;
    }

    public async Task UpdateAsync(ContaPagarReceberUpdateDto dto)
    {
        var model = new Domain.Entities.ContaPagarReceber();

        model.DataAlteracao = DateTime.UtcNow;
        model.Descricao = dto.Descricao;
        model.DataPagamento = dto.DataPagamento;
        model.Valor = dto.Valor;
        model.ContaBancariaId = dto.ContaBancariaId;
        model.Tipo = dto.Tipo;
        model.DataVencimento = dto.DataVencimento;
        model.Status = dto.Status;

        await UpdateAsync(model);
    }

    public async Task UpdateAsync(Domain.Entities.ContaPagarReceber model)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, model.ContaBancariaId, "Conta bancária");

        if (await _repository.GetByIdAsync(model.Id) == null)
            throw new ArgumentException("Conta não encontrada.");

        if (model.Valor <= 0)
            throw new ArgumentException("O valor deve ser maior que zero.");

        if (!model.DataPagamento.HasValue)
            model.Status = StatusContaPagarReceber.Aberta;
        else
            model.Status = StatusContaPagarReceber.Paga;

        model.DataAlteracao = DateTime.UtcNow;
        model.Descricao = model.Descricao;
        model.DataPagamento = model.DataPagamento;
        model.Valor = model.Valor;
        model.ContaBancariaId = model.ContaBancariaId;
        model.Tipo = model.Tipo;
        model.DataVencimento = model.DataVencimento;
        model.Status = model.Status;

        await _repository.UpdateAsync(model);
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
                Tipo = conta.Tipo
            };

            conta.TransacaoId = await _transacaoUseCase.AddAsync(transacao);
        }

        await UpdateAsync(conta);
    }
    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
    public async Task<decimal> GetSaldoPrevistoProximoMes(int mesAtual, int anoAtual, long usuarioId)
    {
        var inicioMesAtual = new DateTime(anoAtual, mesAtual, 1);
        var fimMesAtual    = inicioMesAtual.AddMonths(1).AddDays(-1);
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
            .Where(t => t.DataVencimento >= fimMesAtual && t.DataVencimento <= fimProximoMes && t.UsuarioId == usuarioId && t.DataPagamento == null);

        return await baseQuery.SumAsync(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);

    }
}
