using FinanceControl.Core.Application.UseCases.Usuario;
using FinanceControl.Core.Application.UseCases.ContaBancaria;
using FinanceControl.Core.Application.UseCases.Banco;
using FinanceControl.Core.Application.UseCases.Cartao;
using FinanceControl.Core.Application.UseCases.Fatura;
using FinanceControl.Core.Application.UseCases.Transacao;
using FinanceControl.Core.Application.UseCases.TipoTransacao;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure;
using FinanceControl.Infrastructure.Data;
using FinanceControl.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext com string de conexão
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IContaBancariaRepository, ContaBancariaRepository>();
builder.Services.AddScoped<IBancoRepository, BancoRepository>();
builder.Services.AddScoped<ICartaoRepository, CartaoRepository>();
builder.Services.AddScoped<IFaturaRepository, FaturaRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<ITipoTransacaoRepository, TipoTransacaoRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// UseCases
builder.Services.AddScoped<UsuarioUseCase>();
builder.Services.AddScoped<ContaBancariaUseCase>();
builder.Services.AddScoped<BancoUseCase>();
builder.Services.AddScoped<CartaoUseCase>();
builder.Services.AddScoped<FaturaUseCase>();
builder.Services.AddScoped<TransacaoUseCase>();
builder.Services.AddScoped<TipoTransacaoUseCase>();

// Swagger e CORS
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowAll");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Rotas podem ser mapeadas aqui futuramente

app.Run();
