using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.Transacao;

namespace FinanceControl.Core.Application.UseCases.Transacao;

public class TransacaoUseCase : BaseUseCase, IBaseUseCase<Domain.Entities.Transacao, CreateTransacaoDto, TransacaoResponseDto>
{
    private readonly ITransacaoRepository _repository;
    private readonly ITipoTransacaoRepository _tipoTransacaoRepository;
    private readonly IContaBancariaRepository _contaBancariaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransacaoUseCase(ITransacaoRepository repository, ITipoTransacaoRepository tipoRepository, IContaBancariaRepository contaBancariaRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _tipoTransacaoRepository = tipoRepository;
        _contaBancariaRepository = contaBancariaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TransacaoResponseDto>> GetAllAsync()
    {
        var Transacaos = await _repository.GetAllAsync();
        return Transacaos.Select(u => new TransacaoResponseDto
        {
            Id = u.Id,
            Descricao = u.Descricao,
            Valor = u.Valor,
            DataEfetivacao = u.DataEfetivacao,
            ContaBancariaId = u.ContaBancariaId,
            TipoId = u.TipoId
        });
    }

    public async Task<TransacaoResponseDto?> GetByIdAsync(long id)
    {
        var Transacao = await _repository.GetByIdAsync(id);
        return Transacao is null ? null : new TransacaoResponseDto
        {
            Id = Transacao.Id,
            Descricao = Transacao.Descricao,
            Valor = Transacao.Valor,
            DataEfetivacao = Transacao.DataEfetivacao,
            ContaBancariaId = Transacao.ContaBancariaId,
            TipoId = Transacao.TipoId
        };
    }

    public async Task AddAsync(CreateTransacaoDto dto)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, dto.ContaBancariaId, "Conta bancária");
        await ValidarEntidadeExistenteAsync(_tipoTransacaoRepository, dto.TipoId, "Tipo de transação");

        var Transacao = new Domain.Entities.Transacao
        {
            Descricao = dto.Descricao,
            DataEfetivacao = dto.DataEfetivacao,
            Valor = dto.Valor,
            ContaBancariaId = dto.ContaBancariaId,
            TipoId = dto.TipoId,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        await _repository.AddAsync(Transacao);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(long id, CreateTransacaoDto dto)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, dto.ContaBancariaId, "Conta bancária");
        await ValidarEntidadeExistenteAsync(_tipoTransacaoRepository, dto.TipoId, "Tipo de transação");
        
        var Transacao = await _repository.GetByIdAsync(id);
        if (Transacao == null)
            return;

        Transacao.DataAlteracao = DateTime.UtcNow;
        Transacao.Descricao = dto.Descricao;
        Transacao.DataEfetivacao = dto.DataEfetivacao;
        Transacao.Valor = dto.Valor;
        Transacao.ContaBancariaId = dto.ContaBancariaId;
        Transacao.TipoId = dto.TipoId;

        await _repository.UpdateAsync(Transacao);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}
