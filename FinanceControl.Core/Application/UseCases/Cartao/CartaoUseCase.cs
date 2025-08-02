using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.Cartao;

namespace FinanceControl.Core.Application.UseCases.Cartao;

public class CartaoUseCase : BaseUseCase, IBaseUseCase<Domain.Entities.Cartao, CreateCartaoDto, CartaoResponseDto, CartaoResponseDto>
{
    private readonly ICartaoRepository _repository;
    private readonly IContaBancariaRepository _contaBancariaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CartaoUseCase(ICartaoRepository repository, IContaBancariaRepository contaBancariaRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _contaBancariaRepository = contaBancariaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CartaoResponseDto>> GetAllAsync()
    {
        var Cartaos = await _repository.GetAllAsync();
        return Cartaos.Select(u => new CartaoResponseDto
        {
            Id = u.Id,
            Apelido = u.Apelido,
            ContaBancariaId = u.ContaBancariaId,
            DiaFechamento = u.DiaFechamento,
            Tipo = u.Tipo,
            Limite = u.Limite
        });
    }

    public async Task<CartaoResponseDto?> GetByIdAsync(long id)
    {
        var Cartao = await _repository.GetByIdAsync(id);
        return Cartao is null ? null : new CartaoResponseDto
        {
            Id = Cartao.Id,
            Apelido = Cartao.Apelido,
            ContaBancariaId = Cartao.ContaBancariaId,
            DiaFechamento = Cartao.DiaFechamento,
            Tipo = Cartao.Tipo,
            Limite = Cartao.Limite
        };
    }

    public async Task AddAsync(CreateCartaoDto dto)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, dto.ContaBancariaId, "Conta bancária");

        var Cartao = new Domain.Entities.Cartao
        {
            Apelido = dto.Apelido,
            ContaBancariaId = dto.ContaBancariaId,
            DiaFechamento = dto.DiaFechamento,
            Tipo = dto.Tipo,
            Limite = dto.Limite,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        await _repository.AddAsync(Cartao);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(CartaoResponseDto dto)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, dto.ContaBancariaId, "Conta bancária");
        
        var Cartao = await _repository.GetByIdAsync(dto.Id);
        if (Cartao == null)
            return;

        Cartao.DataAlteracao = DateTime.UtcNow;
        Cartao.Apelido = dto.Apelido;
        Cartao.ContaBancariaId = dto.ContaBancariaId;
        Cartao.DiaFechamento = dto.DiaFechamento;
        Cartao.Tipo = dto.Tipo;
        Cartao.Limite = dto.Limite;

        await _repository.UpdateAsync(Cartao);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
    
}
