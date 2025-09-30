using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Core.Application.DTOs.Transacao;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using ClosedXML.Excel;
using FinanceControl.Core.Domain.Enums;
using System.Globalization;
using FinanceControl.Core.Application.UseCases.Fatura;

namespace FinanceControl.Core.Application.UseCases.Transacao;

public class TransacaoUseCase : BaseUseCase, IBaseUseCase<Domain.Entities.Transacao, TransacaoCreateDto, TransacaoResponseDto, TransacaoUpdateDto>
{
    private readonly ITransacaoRepository _repository;
    private readonly ICategoriaTransacaoRepository _categoriaTransacaoRepository;
    private readonly ICartaoRepository _cartaoTransacaoRepository;
    private readonly IContaBancariaRepository _contaBancariaRepository;
    private readonly IContaPagarReceberRepository _contaPagarReceberRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TransacaoUseCase(
        ITransacaoRepository repository,
        ICategoriaTransacaoRepository categoriaRepository,
        IContaBancariaRepository contaBancariaRepository,
        ICartaoRepository cartaoRepository,
        IFaturaRepository faturaRepository,
        FaturaUseCase faturaUseCase,
        IContaPagarReceberRepository contaPagarReceberRepository,
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _categoriaTransacaoRepository = categoriaRepository;
        _contaBancariaRepository = contaBancariaRepository;
        _cartaoTransacaoRepository = cartaoRepository;
        _contaPagarReceberRepository = contaPagarReceberRepository;
    }

    public async Task<long> AddAsync(TransacaoCreateDto dto)
    {
        await ValidarEntidadeExistenteAsync(_contaBancariaRepository, dto.ContaBancariaId, "Conta bancária");

        if ((int)dto.CategoriaId != 0)
            await ValidarEntidadeExistenteAsync(_categoriaTransacaoRepository, dto.CategoriaId, "Categoria de transação");

        if ((int)(dto.CartaoId ?? 0) != 0)
            await ValidarEntidadeExistenteAsync(_cartaoTransacaoRepository, (long)(dto.CartaoId ?? 0), "Cartão");

        var transacao = new Domain.Entities.Transacao
        {
            Descricao = dto.Descricao,
            DataEfetivacao = dto.DataEfetivacao,
            Valor = Math.Abs(dto.Valor),
            ContaBancariaId = dto.ContaBancariaId,
            CategoriaId = dto.CategoriaId,
            Tipo = dto.Tipo,
            TipoOperacao = dto.TipoOperacao,
            Observacao = dto.Observacao,
            CartaoId = dto.CartaoId,
            FaturaId = dto.FaturaId,
            DataCadastro = DateTime.UtcNow,
            DataAlteracao = DateTime.UtcNow
        };

        if (transacao.Tipo == TipoTransacao.Receita)
            transacao.TipoOperacao = TipoOperacao.Debito;

        await _repository.AddAsync(transacao);
        await _unitOfWork.CommitAsync();

        return transacao.Id;
    }

    public async Task<IEnumerable<TransacaoResponseDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return entities.Select(e => new TransacaoResponseDto(e));
    }

    public async Task<TransacaoResponseDto?> GetByIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : new TransacaoResponseDto(entity);
    }

    public async Task<IEnumerable<TransacaoResponseDto>> GetByFiltroAsync(int mes, int ano, long usuarioId)
    {
        var inicio = new DateTime(ano, mes, 1);
        var fim = inicio.AddMonths(1).AddDays(-1);

        var entities = await _repository.GetAll()
            .Where(t => t.ContaBancaria.UsuarioId == usuarioId &&
                        t.DataEfetivacao >= inicio && t.DataEfetivacao <= fim)
            .AsNoTracking()
            .ToListAsync();

        return entities.Select(e => new TransacaoResponseDto(e));
    }
    public async Task UpdateAsync(TransacaoUpdateDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id);
        if (entity == null) return;

        entity.Descricao = dto.Descricao;
        entity.DataEfetivacao = dto.DataEfetivacao;
        entity.Valor = Math.Abs(dto.Valor);
        entity.ContaBancariaId = dto.ContaBancariaId;
        entity.CategoriaId = dto.CategoriaId;
        entity.Tipo = dto.Tipo;
        entity.Observacao = dto.Observacao;
        entity.CartaoId = dto.CartaoId;
        entity.TipoOperacao = dto.TipoOperacao;
        entity.DataAlteracao = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null) return;

        await _repository.DeleteAsync(entity.Id);
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
                t.TipoOperacao,
                CategoriaNome = t.Categoria.Nome,
                t.ContaBancaria.UsuarioId
            })
            .Where(t => t.UsuarioId == usuarioId && t.TipoOperacao == TipoOperacao.Debito);

        var mesQuery = baseQuery.Where(t => t.DataEfetivacao >= inicioMesAtual && t.DataEfetivacao <= fimMesAtual);

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
                Valor = g.Sum(t => t.Tipo == TipoTransacao.Despesa && t.TipoOperacao != TipoOperacao.Credito ? t.Valor : 0)
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
            .Where(t => t.DataEfetivacao <= fimMesAtual)
            .SumAsync(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);

        var saldoMesAnterior = await baseQuery
            .Where(t => t.DataEfetivacao <= fimMesAnterior)
            .SumAsync(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);

        var saldoPrevistoProximoMes = await baseQuery
            .Where(t => t.DataEfetivacao <= fimProximoMes)
            .SumAsync(t => t.Tipo == TipoTransacao.Receita ? t.Valor : -t.Valor);

        var contas = await _contaPagarReceberRepository.GetAllAsync();
        saldoAtual += contas
            .Where(c => c.DataVencimento <= fimMesAtual && c.DataPagamento == null)
            .Sum(c => c.Tipo == TipoTransacao.Receita ? c.Valor : -c.Valor);

        saldoMesAnterior += contas
            .Where(c => c.DataVencimento <= fimMesAnterior && c.DataPagamento == null)
            .Sum(c => c.Tipo == TipoTransacao.Receita ? c.Valor : -c.Valor);

        saldoPrevistoProximoMes += contas
            .Where(c => c.DataVencimento <= fimProximoMes && c.DataPagamento == null)
            .Sum(c => c.Tipo == TipoTransacao.Receita ? c.Valor : -c.Valor);

        return (
            categorias.Select(x => (x.Categoria, x.Valor)),
            despesas.Select(x => (x.Categoria, x.Valor)),
            receitas.Select(x => (x.Categoria, x.Valor)),
            saldoAtual,
            saldoMesAnterior,
            saldoPrevistoProximoMes
        );
    }
    public async Task<IEnumerable<TransacaoResponseDto>> GetFilteredAsync(TransacaoFilterDto filtro)
    {
        var query = _repository
            .GetAll()
            .Include(t => t.ContaBancaria)
            .Include(t => t.Categoria)
            .AsQueryable();

        if (!string.IsNullOrEmpty(filtro.Descricao))
        {
            query = query.Where(t => t.Descricao.Contains(filtro.Descricao));
        }

        if (filtro.ValorMinimo.HasValue)
        {
            query = query.Where(t => t.Valor >= filtro.ValorMinimo.Value);
        }

        if (filtro.ValorMaximo.HasValue)
        {
            query = query.Where(t => t.Valor <= filtro.ValorMaximo.Value);
        }

        if (filtro.DataInicio.HasValue)
        {
            query = query.Where(t => t.DataEfetivacao >= filtro.DataInicio.Value);
        }

        if (filtro.DataFim.HasValue)
        {
            query = query.Where(t => t.DataEfetivacao <= filtro.DataFim.Value);
        }

        if (filtro.Tipo.HasValue)
        {
            query = query.Where(t => t.Tipo == filtro.Tipo.Value);
        }

        if (filtro.TipoOperacao.HasValue)
        {
            query = query.Where(t => t.TipoOperacao == filtro.TipoOperacao.Value);
        }

        if (filtro.ContaBancariaId.HasValue)
        {
            query = query.Where(t => t.ContaBancariaId == filtro.ContaBancariaId.Value);
        }

        if (filtro.CategoriaId.HasValue)
        {
            query = query.Where(t => t.CategoriaId == filtro.CategoriaId.Value);
        }

        var transacoes = await query.ToListAsync();

        return transacoes.Select(e => new TransacaoResponseDto(e));
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
                CategoriaId = (long)categoriaId,
                TipoOperacao = TipoOperacao.Debito,
            };

            transacoes.Add(transacao);

            await AddAsync(transacao);
        }

        return worksheet.RowsUsed().Count() - 1;
    }
}
