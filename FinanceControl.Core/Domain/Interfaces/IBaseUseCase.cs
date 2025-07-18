namespace FinanceControl.Core.Domain.Interfaces;

public interface IBaseUseCase<TEntity, TCreateDto, TResponseDto>
{
    Task<IEnumerable<TResponseDto>> GetAllAsync();
    Task<TResponseDto?> GetByIdAsync(long id);
    Task AddAsync(TCreateDto dto);
    Task UpdateAsync(long id, TCreateDto dto);
    Task DeleteAsync(long id);
}
