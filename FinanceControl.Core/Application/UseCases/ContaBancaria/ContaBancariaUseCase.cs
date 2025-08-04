using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.ContaBancaria;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Core.Application.UseCases.ContaBancaria;

public class ContaBancariaUseCase : BaseUseCase, IBaseUseCase<Domain.Entities.ContaBancaria, ContaBancariaCreateDto, ContaBancariaResponseDto, ContaBancariaUpdateDto>
{
    private readonly IContaBancariaRepository _repository;
    private readonly IBancoRepository _bancoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ContaBancariaUseCase(IContaBancariaRepository repository, IBancoRepository bancoRepository, IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _bancoRepository = bancoRepository;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ContaBancariaResponseDto>> GetAllAsync()
    {
        var contas = await _repository
            .GetAll()
            .Include(c => c.Banco)
            .ToListAsync();

        return contas.Select(c => new ContaBancariaResponseDto
        {
            Id = c.Id,
            Numero = c.Numero,
            BancoId = c.BancoId,
            UsuarioId = c.UsuarioId,
            BancoNome = c.Banco.Nome,
        });
    }

    public async Task<ContaBancariaResponseDto?> GetByIdAsync(long id)
    {
        var contas = await _repository.GetByIdAsync(id);
        return contas is null ? null : new ContaBancariaResponseDto
        {
            Id = contas.Id,
            Numero = contas.Numero,
            UsuarioId = contas.UsuarioId,
            BancoId = contas.BancoId
        };
    }

    public async Task AddAsync(ContaBancariaCreateDto dto)
    {
        await ValidarEntidadeExistenteAsync(_usuarioRepository, dto.UsuarioId, "Usuário");
        await ValidarEntidadeExistenteAsync(_bancoRepository, dto.BancoId, "Banco");

        var contas = new Domain.Entities.ContaBancaria
        {
            Numero = dto.Numero,
            UsuarioId = dto.UsuarioId,
            BancoId = dto.BancoId,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        await _repository.AddAsync(contas);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(ContaBancariaUpdateDto dto)
    {
        await ValidarEntidadeExistenteAsync(_usuarioRepository, dto.UsuarioId, "Usuário");
        await ValidarEntidadeExistenteAsync(_bancoRepository, dto.BancoId, "Banco");
        
        var contas = await _repository.GetByIdAsync(dto.Id);
        if (contas == null)
            return;

        contas.DataAlteracao = DateTime.UtcNow;
        contas.Numero = dto.Numero;
        contas.UsuarioId = dto.UsuarioId;
        contas.BancoId = dto.BancoId;

        await _repository.UpdateAsync(contas);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}
