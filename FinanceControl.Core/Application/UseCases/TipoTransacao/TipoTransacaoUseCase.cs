using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.TipoTransacao;

namespace FinanceControl.Core.Application.UseCases.TipoTransacao;

public class TipoTransacaoUseCase : IBaseUseCase<Domain.Entities.TipoTransacao, CreateTipoTransacaoDto, TipoTransacaoResponseDto>
{
    private readonly ITipoTransacaoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TipoTransacaoUseCase(ITipoTransacaoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TipoTransacaoResponseDto>> GetAllAsync()
    {
        var TipoTransacaos = await _repository.GetAllAsync();
        return TipoTransacaos.Select(u => new TipoTransacaoResponseDto
        {
            Id = u.Id,
            Nome = u.Nome
        });
    }

    public async Task<TipoTransacaoResponseDto?> GetByIdAsync(long id)
    {
        var TipoTransacao = await _repository.GetByIdAsync(id);
        return TipoTransacao is null ? null : new TipoTransacaoResponseDto
        {
            Id = TipoTransacao.Id,
            Nome = TipoTransacao.Nome
        };
    }

    public async Task AddAsync(CreateTipoTransacaoDto dto)
    {
        var TipoTransacao = new Domain.Entities.TipoTransacao
        {
            Nome = dto.Nome
        };

        await _repository.AddAsync(TipoTransacao);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(long id, CreateTipoTransacaoDto dto)
    {
        var TipoTransacao = await _repository.GetByIdAsync(id);
        if (TipoTransacao == null)
            return;

        TipoTransacao.DataAlteracao = DateTime.UtcNow;
        TipoTransacao.Nome = dto.Nome;

        await _repository.UpdateAsync(TipoTransacao);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}
