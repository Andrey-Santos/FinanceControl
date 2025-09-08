using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.Transacao;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ClosedXML.Excel;
using FinanceControl.Core.Domain.Enums;
using System.Globalization;

namespace FinanceControl.Core.Application.UseCases.Transacao;

public class TransacaoUseCase : BaseUseCase, IBaseUseCase<Domain.Entities.Transacao, TransacaoCreateDto, TransacaoResponseDto, TransacaoUpdateDto>
{
    private readonly ITransacaoRepository _repository;
    private readonly ICategoriaTransacaoRepository _categoriaTransacaoRepository;
    private readonly IContaBancariaRepository _contaBancariaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransacaoUseCase(ITransacaoRepository repository, ICategoriaTransacaoRepository categoriaRepository, IContaBancariaRepository contaBancariaRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _categoriaTransacaoRepository = categoriaRepository;
        _contaBancariaRepository = contaBancariaRepository;
    }

    public async Task<IEnumerable<TransacaoResponseDto>> GetByFiltroAsync(int mesAtual, int anoAtual, long usuarioId)
    {
        var query = _repository.GetAll();

        var inicioMesAtual = new DateTime(anoAtual, mesAtual, 1);
        var fimMesAtual = inicioMesAtual.AddMonths(1).AddDays(-1);

        query = query.Where(t => t.DataEfetivacao >= inicioMesAtual);
        query = query.Where(t => t.DataEfetivacao <= fimMesAtual);
        query = query.Where(t => t.ContaBancaria.UsuarioId == usuarioId);

        var transacoes = await query
            .AsNoTracking()
            .ToListAsync();

        return transacoes.Select(t => new TransacaoResponseDto
        {
            Id = t.Id,
            Descricao = t.Descricao,
            Valor = t.Valor,
            DataEfetivacao = t.DataEfetivacao,
            Tipo = t.Tipo,
            ContaBancariaId = t.ContaBancariaId
        });
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
            CategoriaNome = u.Categoria.Nome,
            Tipo = u.Tipo
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
            CategoriaId = Transacao.CategoriaId,
            Tipo = Transacao.Tipo
        };
    }

    public async Task<long> AddAsync(TransacaoCreateDto dto)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, dto.ContaBancariaId, "Conta bancária");

        if ((int)dto.CategoriaId != 0)
            await ValidarEntidadeExistenteAsync(_categoriaTransacaoRepository, dto.CategoriaId, "Categoria de transação");

        var transacao = new Domain.Entities.Transacao
        {
            Descricao = dto.Descricao,
            DataEfetivacao = DateTime.SpecifyKind(dto.DataEfetivacao, DateTimeKind.Utc),
            Valor = Math.Abs(dto.Valor),
            ContaBancariaId = dto.ContaBancariaId,
            CategoriaId = dto.CategoriaId,
            Tipo = dto.Tipo,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        await _repository.AddAsync(transacao);
        await _unitOfWork.CommitAsync();

        return transacao.Id;
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
        Transacao.Tipo = dto.Tipo;

        await _repository.UpdateAsync(Transacao);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        await _repository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }

    public async Task<(IEnumerable<(string Categoria, decimal Valor)> categorias,
        IEnumerable<(string Categoria, decimal Valor)> despesas,
        IEnumerable<(string Categoria, decimal Valor)> receitas,
        decimal saldoAtual,
        decimal saldoMesAnterior,
        decimal saldoPrevistoProximoMes)> GetResumoDashboardAsync(int mesAtual, int anoAtual, long usuarioId)
    {
        var inicioMesAtual = new DateTime(anoAtual, mesAtual, 1);
        var fimMesAtual = inicioMesAtual.AddMonths(1).AddDays(-1);
        var fimMesAnterior = inicioMesAtual.AddDays(-1);
        var fimProximoMes = inicioMesAtual.AddMonths(2).AddDays(-1);

        var baseQuery = _repository
            .GetAll()
            .AsNoTracking()
            .Select(t => new    
            {
                t.DataEfetivacao,
                t.Valor,
                t.Tipo,
                CategoriaNome = t.Categoria.Nome,
                t.ContaBancaria.UsuarioId
            });

        var mesQuery = baseQuery.Where(t => t.DataEfetivacao >= inicioMesAtual && t.DataEfetivacao <= fimMesAtual && t.UsuarioId == usuarioId);

        var categoriasQuery = mesQuery
            .GroupBy(t => t.CategoriaNome)
            .Select(g => new
            {
                Categoria = g.Key,
                Valor = g.Sum(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor)
            })
            .Where(x => x.Valor != 0);

        var despesasQuery = mesQuery
            .GroupBy(t => t.CategoriaNome)
            .Select(g => new
            {
                Categoria = g.Key,
                Valor = g.Sum(t => t.Tipo == TipoTransacao.Despesa ? t.Valor : 0)
            })
            .Where(x => x.Valor != 0);

        var receitasQuery = mesQuery
            .GroupBy(t => t.CategoriaNome)
            .Select(g => new
            {
                Categoria = g.Key,
                Valor = g.Sum(t => t.Tipo == TipoTransacao.Receita ? t.Valor : 0)
            })
            .Where(x => x.Valor != 0);

        var categorias = await categoriasQuery.ToListAsync();
        var despesas = await despesasQuery.ToListAsync();
        var receitas = await receitasQuery.ToListAsync();

        var saldoAtual = await baseQuery
            .Where(t => t.UsuarioId == usuarioId && t.DataEfetivacao <= fimMesAtual)
            .SumAsync(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);

        var saldoMesAnterior = await baseQuery
            .Where(t => t.UsuarioId == usuarioId && t.DataEfetivacao <= fimMesAnterior)
            .SumAsync(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);

        var saldoPrevistoProximoMes = await baseQuery
            .Where(t => t.UsuarioId == usuarioId && t.DataEfetivacao <= fimProximoMes)
            .SumAsync(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);

        return (
            categorias.Select(x => (x.Categoria, x.Valor)),
            despesas.Select(x => (x.Categoria, x.Valor)),
            receitas.Select(x => (x.Categoria, x.Valor)),
            saldoAtual,
            saldoMesAnterior,
            saldoPrevistoProximoMes
        );
    }

    public async Task<int> ImportarAsync(IFormFile arquivo, long contaBancariaId)
    {
        if (arquivo == null || arquivo.Length == 0)
            throw new ArgumentException("Nenhum arquivo selecionado.");

        if (!Path.GetExtension(arquivo.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Formato inválido. Apenas arquivos .xlsx são permitidos.");

        using var stream = new MemoryStream();
        await arquivo.CopyToAsync(stream);
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheet(1);

        var transacoes = new List<TransacaoCreateDto>();

        foreach (var row in worksheet.RowsUsed().Skip(1))
        {
            string categoriaNome = row.Cell(4).GetString();
            long? categoriaId = _categoriaTransacaoRepository.GetByNomeAsync(categoriaNome).Result?.Id ?? _categoriaTransacaoRepository.GetByNomeAsync("Outros").Result?.Id;

            if (categoriaId == null)
            {
                var novaCategoria = new Domain.Entities.CategoriaTransacao
                {
                    Nome = "Outros",
                    DataCadastro = DateTime.UtcNow,
                    DataAlteracao = DateTime.UtcNow
                };
                await _categoriaTransacaoRepository.AddAsync(novaCategoria);
                await _unitOfWork.CommitAsync();
                categoriaId = novaCategoria.Id;
            }

            var valorStr = row.Cell(3).GetValue<string>();
            decimal valor;

            if (!decimal.TryParse(valorStr, NumberStyles.Any, new CultureInfo("pt-BR"), out valor))
                throw new Exception($"Valor inválido na célula {row.Cell(3).Address}: {valorStr}");

            TransacaoCreateDto transacao = new TransacaoCreateDto
            {
                DataEfetivacao = row.Cell(1).GetDateTime(),
                Descricao = row.Cell(2).GetString(),
                Valor = valor,
                ContaBancariaId = contaBancariaId,
                Tipo = row.Cell(3).GetValue<decimal>() < 0 ? TipoTransacao.Despesa : TipoTransacao.Receita,
                CategoriaId = (long)categoriaId
            };
            
            transacoes.Add(transacao);

            await AddAsync(transacao);
        }

        return worksheet.RowsUsed().Count() - 1;
    }
}
