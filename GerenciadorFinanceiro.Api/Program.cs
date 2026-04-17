using GerenciadorFinanceiro.Api.Middleware;
using GerenciadorFinanceiro.Application;
using GerenciadorFinanceiro.Infrastructure;
using GerenciadorFinanceiro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurando o banco de dados PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona os serviços para os Controllers da API
builder.Services.AddControllers();

// Adiciona o Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do CORS
builder.Services.AddCors(options => options.AddPolicy("AngularDev", policy => policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()));

// Configura Injeção de Dependência das camadas
builder.Services.AddInfrastructure();
builder.Services.AddApplication();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configura o ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AngularDev");
app.UseAuthorization();

// Permite servir o frontend (index.html, js, css) da pasta wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();

// Garante que rotas do Angular (ex: /transacoes) sejam redirecionadas para o index.html
app.MapFallbackToFile("index.html");

app.Run();
