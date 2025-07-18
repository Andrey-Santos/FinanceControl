using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.Cartao;

namespace FinanceControl.Core.Application.UseCases.Cartao;

public class CartaoUseCase : IBaseUseCase<Domain.Entities.Cartao, CreateCartaoDto, CartaoResponseDto>
{
    private readonly ICartaoRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CartaoUseCase(ICartaoRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CartaoResponseDto>> GetAllAsync()
    {
        var Cartaos = await _repository.GetAllAsync();
        return Cartaos.Select(u => new CartaoResponseDto
        {
            Id = u.Id,
            Apelido = u.Apelido,
            BancoId = u.BancoId,
            DataFechamento = u.DataFechamento,
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
            BancoId = Cartao.BancoId,
            DataFechamento = Cartao.DataFechamento,
            Tipo = Cartao.Tipo,
            Limite = Cartao.Limite
        };
    }

    public async Task AddAsync(CreateCartaoDto dto)
    {
        var Cartao = new Domain.Entities.Cartao
        {
            Apelido = dto.Apelido,
            BancoId = dto.BancoId,
            DataFechamento = dto.DataFechamento,
            Tipo = dto.Tipo,
            Limite = dto.Limite,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        await _repository.AddAsync(Cartao);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(long id, CreateCartaoDto dto)
    {
        var Cartao = await _repository.GetByIdAsync(id);
        if (Cartao == null)
            return;

        Cartao.DataAlteracao = DateTime.UtcNow;
        Cartao.Apelido = dto.Apelido;
        Cartao.BancoId = dto.BancoId;
        Cartao.DataFechamento = dto.DataFechamento;
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
