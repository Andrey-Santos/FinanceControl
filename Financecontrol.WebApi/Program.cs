using FinanceControl.Core.Application.UseCases.Usuario;
using FinanceControl.Core.Application.UseCases.ContaBancaria;
using FinanceControl.Core.Application.UseCases.Banco;
using FinanceControl.Core.Application.UseCases.Cartao;
using FinanceControl.Core.Application.UseCases.Fatura;
using FinanceControl.Core.Application.UseCases.Transacao;
using FinanceControl.Core.Application.UseCases.CategoriaTransacao;
using FinanceControl.Core.Domain.Interfaces;
using FinanceControl.Infrastructure;
using FinanceControl.Infrastructure.Data;
using FinanceControl.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FinanceControl.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.OpenApi.Models;
using FinanceControl.Core.Application.UseCases.ContaPagarReceber;
using Financecontrol.WebApi.Filters;

var builder = WebApplication.CreateBuilder(args);

// 🔒 Impede renomeação automática dos claims
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// 🔌 Conexão com banco de dados
builder.Services.AddDbContext<FinanceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 📦 Repositórios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IContaBancariaRepository, ContaBancariaRepository>();
builder.Services.AddScoped<IBancoRepository, BancoRepository>();
builder.Services.AddScoped<ICartaoRepository, CartaoRepository>();
builder.Services.AddScoped<IFaturaRepository, FaturaRepository>();
builder.Services.AddScoped<ITransacaoRepository, TransacaoRepository>();
builder.Services.AddScoped<ICategoriaTransacaoRepository, CategoriaTransacaoRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IContaPagarReceberRepository, ContaPagarReceberRepository>();

// ⚙️ UseCases
builder.Services.AddScoped<UsuarioUseCase>();
builder.Services.AddScoped<ContaBancariaUseCase>();
builder.Services.AddScoped<BancoUseCase>();
builder.Services.AddScoped<CartaoUseCase>();
builder.Services.AddScoped<FaturaUseCase>();
builder.Services.AddScoped<TransacaoUseCase>();
builder.Services.AddScoped<CategoriaTransacaoUseCase>();
builder.Services.AddScoped<ContaPagarReceberUseCase>();

// 🔐 JWT
var jwtConfig = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtConfig["Key"];

if (string.IsNullOrEmpty(jwtKey))
    throw new InvalidOperationException("JWT Key is missing in configuration.");

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Lê o token do cookie
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["jwt"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };

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

// 🌐 HTTP Client para chamadas internas
builder.Services.AddHttpClient();

// 🧭 MVC e Views
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<CustomExceptionFilter>();
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login/Index"; // Caminho da página de login
});

// 🔓 CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// 📘 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "FinanceControl API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Digite o token JWT no campo abaixo",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

// 🧭 Middlewares
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 🌐 Roteamento
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
