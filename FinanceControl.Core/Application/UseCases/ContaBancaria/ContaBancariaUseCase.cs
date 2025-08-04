using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.ContaBancaria;

namespace FinanceControl.Core.Application.UseCases.ContaBancaria;

public class ContaBancariaUseCase : BaseUseCase, IBaseUseCase<Domain.Entities.ContaBancaria, ContaBancariaCreateDto, ContaBancariaResponseDto, ContaBancariaResponseDto>
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
        var ContaBancarias = await _repository.GetAllAsync();
        return ContaBancarias.Select(u => new ContaBancariaResponseDto
        {
            Id = u.Id,
            Numero = u.Numero,
            UsuarioId = u.UsuarioId,
            BancoId = u.BancoId
        });
    }

    public async Task<ContaBancariaResponseDto?> GetByIdAsync(long id)
    {
        var ContaBancaria = await _repository.GetByIdAsync(id);
        return ContaBancaria is null ? null : new ContaBancariaResponseDto
        {
            Id = ContaBancaria.Id,
            Numero = ContaBancaria.Numero,
            UsuarioId = ContaBancaria.UsuarioId,
            BancoId = ContaBancaria.BancoId
        };
    }

    public async Task AddAsync(ContaBancariaCreateDto dto)
    {
        await ValidarEntidadeExistenteAsync(_usuarioRepository, dto.UsuarioId, "Usuário");
        await ValidarEntidadeExistenteAsync(_bancoRepository, dto.BancoId, "Banco");

        var ContaBancaria = new Domain.Entities.ContaBancaria
        {
            Numero = dto.Numero,
            UsuarioId = dto.UsuarioId,
            BancoId = dto.BancoId,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        await _repository.AddAsync(ContaBancaria);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(ContaBancariaResponseDto dto)
    {
        await ValidarEntidadeExistenteAsync(_usuarioRepository, dto.UsuarioId, "Usuário");
        await ValidarEntidadeExistenteAsync(_bancoRepository, dto.BancoId, "Banco");
        
        var ContaBancaria = await _repository.GetByIdAsync(dto.Id);
        if (ContaBancaria == null)
            return;

        ContaBancaria.DataAlteracao = DateTime.UtcNow;
        ContaBancaria.Numero = dto.Numero;
        ContaBancaria.UsuarioId = dto.UsuarioId;
        ContaBancaria.BancoId = dto.BancoId;

        await _repository.UpdateAsync(ContaBancaria);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}
