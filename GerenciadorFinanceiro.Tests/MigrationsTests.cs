using GerenciadorFinanceiro.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Tests
{
    public class MigrationsTests
    {
        [Fact]
        public void Migrations_CreateTransacoesTable_WithExpectedColumns()
        {
            // Arrange: create SQLite in-memory connection
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
            .Options;

            // Act: apply migrations
            using (var context = new AppDbContext(options))
            {
                context.Database.Migrate();
            }

            // Inspect schema
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "PRAGMA table_info('Transacoes');";

            var columns = new List<(string name, string type, string? dflt_value, int notnull)>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    columns.Add((
                    name: reader.GetString(reader.GetOrdinal("name")),
                    type: reader.GetString(reader.GetOrdinal("type")),
                    dflt_value: reader.IsDBNull(reader.GetOrdinal("dflt_value")) ? null : reader.GetString(reader.GetOrdinal("dflt_value")),
                    notnull: reader.GetInt32(reader.GetOrdinal("notnull"))));
                }
            }

            // Assert: expected columns exist
            var expected = new[] { "Id", "Data", "Descricao", "Valor", "Tipo", "CategoriaId", "ContaBancariaId", "CartaoCreditoId", "Categoria", "NomeCartao", "FinalCartao", "Parcela", "Cotacao" };
            foreach (var col in expected)
            {
                Assert.Contains(columns, c => string.Equals(c.name, col, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Fact]
        public void Migrations_ValorAndCotacao_TypesAndDefaultsAreNumeric()
        {
            // Arrange
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning))
            .Options;

            using (var context = new AppDbContext(options))
            {
                context.Database.Migrate();
            }

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "PRAGMA table_info('Transacoes');";

            var cols = new Dictionary<string, (string type, string? dflt_value, int notnull)>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var name = reader.GetString(reader.GetOrdinal("name"));
                    var type = reader.GetString(reader.GetOrdinal("type"));
                    var dflt = reader.IsDBNull(reader.GetOrdinal("dflt_value")) ? null : reader.GetString(reader.GetOrdinal("dflt_value"));
                    var notnull = reader.GetInt32(reader.GetOrdinal("notnull"));
                    cols[name] = (type, dflt, notnull);
                }
            }

            Assert.True(cols.ContainsKey("Valor"), "Coluna 'Valor' não encontrada");
            Assert.True(cols.ContainsKey("Cotacao"), "Coluna 'Cotacao' não encontrada");

            var valorType = cols["Valor"].type?.ToLowerInvariant() ?? string.Empty;
            var cotacaoType = cols["Cotacao"].type?.ToLowerInvariant() ?? string.Empty;

            // Accept several possible numeric type strings depending on provider
            bool valorIsNumeric = valorType.Contains("decimal") || valorType.Contains("numeric") || valorType.Contains("real") || valorType.Contains("double") || valorType.Contains("int");
            bool cotacaoIsNumeric = cotacaoType.Contains("decimal") || cotacaoType.Contains("numeric") || cotacaoType.Contains("real") || cotacaoType.Contains("double") || cotacaoType.Contains("int");

            Assert.True(valorIsNumeric, $"Tipo da coluna Valor inesperado: {valorType}");
            Assert.True(cotacaoIsNumeric, $"Tipo da coluna Cotacao inesperado: {cotacaoType}");

            // Defaults: Categoria should have default empty string and Cotacao default should include 0
            string? categoriaDflt = null;
            if (cols.TryGetValue("Categoria", out var categoriaInfo))
            {
                categoriaDflt = categoriaInfo.dflt_value;
            }

            string? cotacaoDflt = null;
            if (cols.TryGetValue("Cotacao", out var cotacaoInfo))
            {
                cotacaoDflt = cotacaoInfo.dflt_value;
            }

            Assert.True(categoriaDflt != null && (categoriaDflt.Contains("''") || categoriaDflt.Contains("\"\"") || string.IsNullOrWhiteSpace(categoriaDflt.Trim())), $"Default de Categoria inesperado: {categoriaDflt}");
            Assert.True(cotacaoDflt != null && cotacaoDflt.Contains('0'), $"Default de Cotacao inesperado: {cotacaoDflt}");
        }
    }
}
