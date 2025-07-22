using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.Banco;

namespace FinanceControl.Core.Application.UseCases.Banco;

public class BancoUseCase : BaseUseCase, IBaseUseCase<Domain.Entities.Banco, CreateBancoDto, BancoResponseDto>
{
    private readonly IBancoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public BancoUseCase(IBancoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BancoResponseDto>> GetAllAsync()
    {
        var Bancos = await _repository.GetAllAsync();
        return Bancos.Select(u => new BancoResponseDto
        {
            Id = u.Id,
            Nome = u.Nome
        });
    }

    public async Task<BancoResponseDto?> GetByIdAsync(long id)
    {
        var Banco = await _repository.GetByIdAsync(id);
        return Banco is null ? null : new BancoResponseDto
        {
            Id = Banco.Id,
            Nome = Banco.Nome
        };
    }

    public async Task AddAsync(CreateBancoDto dto)
    {
        var Banco = new Domain.Entities.Banco
        {
            Nome = dto.Nome
        };

        await _repository.AddAsync(Banco);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(long id, CreateBancoDto dto)
    {
        var Banco = await _repository.GetByIdAsync(id);
        if (Banco == null)
            return;

        Banco.DataAlteracao = DateTime.UtcNow;
        Banco.Nome = dto.Nome;

        await _repository.UpdateAsync(Banco);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}
