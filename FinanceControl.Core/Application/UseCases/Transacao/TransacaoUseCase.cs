using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.Transacao;
using Microsoft.EntityFrameworkCore;

namespace FinanceControl.Core.Application.UseCases.Transacao;

public class TransacaoUseCase : BaseUseCase, IBaseUseCase<Domain.Entities.Transacao, TransacaoCreateDto, TransacaoResponseDto, TransacaoUpdateDto>
{
    private readonly ITransacaoRepository _repository;
    private readonly ICategoriaTransacaoRepository _categoriaTransacaoRepository;
    private readonly IContaBancariaRepository _contaBancariaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransacaoUseCase(ITransacaoRepository repository, ICategoriaTransacaoRepository categoriaRepository, IContaBancariaRepository contaBancariaRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _categoriaTransacaoRepository = categoriaRepository;
        _contaBancariaRepository = contaBancariaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TransacaoResponseDto>> GetAllAsync()
    {
        var Transacaos = await _repository
                                .GetAll()
                                .Include(t => t.ContaBancaria)
                                .Include(t => t.Categoria)
                                .ToListAsync();

        return Transacaos.Select(u => new TransacaoResponseDto
        {
            Id = u.Id,
            Descricao = u.Descricao,
            Valor = u.Valor,
            DataEfetivacao = u.DataEfetivacao,
            ContaBancariaId = u.ContaBancariaId,
            CategoriaId = u.CategoriaId,
            ContaBancariaNumero = u.ContaBancaria.Numero,
            CategoriaNome = u.Categoria.Nome
        });
    }

    public async Task<TransacaoResponseDto?> GetByIdAsync(long id)
    {
        var Transacao = await _repository.GetByIdAsync(id);
        return Transacao is null ? null : new TransacaoResponseDto
        {
            Id = Transacao.Id,
            Descricao = Transacao.Descricao,
            Valor = Transacao.Valor,
            DataEfetivacao = Transacao.DataEfetivacao,
            ContaBancariaId = Transacao.ContaBancariaId,
            CategoriaId = Transacao.CategoriaId
        };
    }

    public async Task AddAsync(TransacaoCreateDto dto)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, dto.ContaBancariaId, "Conta bancária");
        await ValidarEntidadeExistenteAsync(_categoriaTransacaoRepository, dto.CategoriaId, "Categoria de transação");

        var Transacao = new Domain.Entities.Transacao
        {
            Descricao = dto.Descricao,
            DataEfetivacao = dto.DataEfetivacao,
            Valor = dto.Valor,
            ContaBancariaId = dto.ContaBancariaId,
            CategoriaId = dto.CategoriaId,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        await _repository.AddAsync(Transacao);
        await _unitOfWork.CommitAsync();
    }

    public async Task UpdateAsync(TransacaoUpdateDto dto)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, dto.ContaBancariaId, "Conta bancária");
        await ValidarEntidadeExistenteAsync(_categoriaTransacaoRepository, dto.CategoriaId, "Categoria de transação");
        
        var Transacao = await _repository.GetByIdAsync(dto.Id);
        if (Transacao == null)
            return;

        Transacao.DataAlteracao = DateTime.UtcNow;
        Transacao.Descricao = dto.Descricao;
        Transacao.DataEfetivacao = dto.DataEfetivacao;
        Transacao.Valor = dto.Valor;
        Transacao.ContaBancariaId = dto.ContaBancariaId;
        Transacao.CategoriaId = dto.CategoriaId;

        await _repository.UpdateAsync(Transacao);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }
}
