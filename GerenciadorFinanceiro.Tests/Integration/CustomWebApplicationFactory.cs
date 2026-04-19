using GerenciadorFinanceiro.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GerenciadorFinanceiro.Tests.Integration
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
        where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                // 1. Criar e abrir uma conexão SQLite em memória que persistirá durante o tempo de vida da factory
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                // 2. Adicionar o DbContext usando SQLite
                services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));

                // 3. Garantir que o banco de dados é criado (aplicando schema)
                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            });
        }
    }
}
