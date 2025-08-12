using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.CategoriaTransacao;

namespace FinanceControl.Core.Application.UseCases.CategoriaTransacao;

public class CategoriaTransacaoUseCase : IBaseUseCase<Domain.Entities.CategoriaTransacao, CategoriaTransacaoCreateDto, CategoriaTransacaoResponseDto, CategoriaTransacaoUpdateDto>
{
    private readonly ICategoriaTransacaoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoriaTransacaoUseCase(ICategoriaTransacaoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CategoriaTransacaoResponseDto>> GetAllAsync()
    {
        var CategoriaTransacaos = await _repository.GetAllAsync();
        return CategoriaTransacaos.Select(u => new CategoriaTransacaoResponseDto
        {
            Id = u.Id,
            Nome = u.Nome
        });
    }

    public async Task<CategoriaTransacaoResponseDto?> GetByIdAsync(long id)
    {
        var CategoriaTransacao = await _repository.GetByIdAsync(id);
        return CategoriaTransacao is null ? null : new CategoriaTransacaoResponseDto
        {
            Id = CategoriaTransacao.Id,
            Nome = CategoriaTransacao.Nome
        };
    }

    public async Task AddAsync(CategoriaTransacaoCreateDto dto)
    {
        var CategoriaTransacao = new Domain.Entities.CategoriaTransacao
        {
            Nome = dto.Nome,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        await _repository.AddAsync(CategoriaTransacao);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(CategoriaTransacaoUpdateDto dto)
    {
        var CategoriaTransacao = await _repository.GetByIdAsync(dto.Id);
        if (CategoriaTransacao == null)
            return;

        CategoriaTransacao.DataAlteracao = DateTime.UtcNow;
        CategoriaTransacao.Nome = dto.Nome;

        await _repository.UpdateAsync(CategoriaTransacao);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}
