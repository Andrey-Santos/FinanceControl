namespace FinanceControl.Core.Domain.Interfaces;

public interface IBaseUseCase<TEntity, TCreateDto, TResponseDto, TUpdateDto>
    where TEntity : class
    where TCreateDto : class
    where TResponseDto : class
    where TUpdateDto : class
{
    Task<IEnumerable<TResponseDto>> GetAllAsync();
    Task<TResponseDto?> GetByIdAsync(long id);
    Task<long> AddAsync(TCreateDto dto);
    Task UpdateAsync(TUpdateDto dto);
    Task DeleteAsync(long id);
}
