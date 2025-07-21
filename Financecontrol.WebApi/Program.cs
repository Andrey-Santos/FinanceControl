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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FinanceControl.Infrastructure.Services;
using System.Text;

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

// JWT Token Service
var jwtConfig = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtConfig["Key"];

if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("JWT Key is missing in configuration.");

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true
        };
    });

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowAll");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

// Rotas podem ser mapeadas aqui futuramente

app.Run();
