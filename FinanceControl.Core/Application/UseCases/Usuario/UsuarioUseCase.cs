using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.Usuario;

namespace FinanceControl.Core.Application.UseCases.Usuario;

public class UsuarioUseCase : IBaseUseCase<Domain.Entities.Usuario, CreateUsuarioDto, UsuarioResponseDto>
{
    private readonly IUsuarioRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UsuarioUseCase(IUsuarioRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UsuarioResponseDto>> GetAllAsync()
    {
        var usuarios = await _repository.GetAllAsync();
        return usuarios.Select(u => new UsuarioResponseDto
        {
            Id = u.Id,
            Nome = u.Nome
        });
    }

    public async Task<UsuarioResponseDto?> GetByIdAsync(long id)
    {
        var usuario = await _repository.GetByIdAsync(id);
        return usuario is null ? null : new UsuarioResponseDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome
        };
    }

    public async Task AddAsync(CreateUsuarioDto dto)
    {
        var usuario = new Domain.Entities.Usuario
        {
            Nome = dto.Nome,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        await _repository.AddAsync(usuario);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(long id, CreateUsuarioDto dto)
    {
        var usuario = await _repository.GetByIdAsync(id);
        if (usuario == null)
            return;

        usuario.Nome = dto.Nome;
        usuario.DataAlteracao = DateTime.UtcNow;

        await _repository.UpdateAsync(usuario);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}
