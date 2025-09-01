using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.Fatura;

namespace FinanceControl.Core.Application.UseCases.Fatura;

public class FaturaUseCase : BaseUseCase, IBaseUseCase<Domain.Entities.Fatura, CreateFaturaDto, FaturaResponseDto, FaturaResponseDto>
{
    private readonly IFaturaRepository _repository;
    private readonly ICartaoRepository _cartaoRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FaturaUseCase(IFaturaRepository repository, ICartaoRepository cartaoRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _cartaoRepository = cartaoRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<FaturaResponseDto>> GetAllAsync()
    {
        var Faturas = await _repository.GetAllAsync();
        return Faturas.Select(u => new FaturaResponseDto
        {
            Id = u.Id,
            Mes = u.Mes,
            Ano = u.Ano,
            CartaoId = u.CartaoId
        });
    }

    public async Task<FaturaResponseDto?> GetByIdAsync(long id)
    {
        var Fatura = await _repository.GetByIdAsync(id);
        return Fatura is null ? null : new FaturaResponseDto
        {
            Id = Fatura.Id,
            Mes = Fatura.Mes,
            Ano = Fatura.Ano,
            CartaoId = Fatura.CartaoId
        };
    }

    public async Task<long> AddAsync(CreateFaturaDto dto)
    {
        await ValidarEntidadeExistenteAsync(_cartaoRepository, dto.CartaoId, "Cartão");

        var Fatura = new Domain.Entities.Fatura
        {
            CartaoId = dto.CartaoId,
            Mes = dto.Mes,
            Ano = dto.Ano,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        await _repository.AddAsync(Fatura);
        await _unitOfWork.CommitAsync();
        return Fatura.Id;
    }

    public async Task UpdateAsync(FaturaResponseDto dto)
    {
        await ValidarEntidadeExistenteAsync(_cartaoRepository, dto.CartaoId, "Cartão");

        var Fatura = await _repository.GetByIdAsync(dto.Id);
        if (Fatura == null)
            return;

        Fatura.DataAlteracao = DateTime.UtcNow;
        Fatura.CartaoId = dto.CartaoId;
        Fatura.Mes = dto.Mes;
        Fatura.Ano = dto.Ano;
        Fatura.DataAlteracao = DateTime.UtcNow;

        await _repository.UpdateAsync(Fatura);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}
