using GerenciadorFinanceiro.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Transacao> Transacoes { get; set; } = null!;
        public DbSet<ContaBancaria> ContasBancarias { get; set; } = null!;
        public DbSet<CartaoCredito> CartoesDeCredito { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração Global para DateTime (PostgreSQL UTC Fix)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var properties = entityType.GetProperties()
                    .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));

                foreach (var property in properties)
                {
                    property.SetValueConverter(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
            }

            modelBuilder.Entity<Transacao>(entity =>
            {
                entity.ToTable("Transacoes");

                entity.Property(t => t.Descricao)
                    .HasMaxLength(200)
                    .IsRequired();

                // Mapeamento de valores numéricos com precisão
                entity.Property(t => t.Valor)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(t => t.Cotacao)
                    .HasPrecision(18, 2)
                    .HasDefaultValue(0m);

                // Campos de texto adicionais
                entity.Property(t => t.Categoria)
                    .HasMaxLength(150)
                    .HasDefaultValue(string.Empty);

                entity.Property(t => t.NomeCartao)
                    .HasMaxLength(150)
                    .HasDefaultValue(string.Empty);

                entity.Property(t => t.FinalCartao)
                    .HasMaxLength(20)
                    .HasDefaultValue(string.Empty);

                entity.Property(t => t.Parcela)
                    .HasMaxLength(50)
                    .HasDefaultValue(string.Empty);

                // Chaves e relacionamentos já são inferidos pelas propriedades existentes
            });

            modelBuilder.Entity<ContaBancaria>()
                .ToTable("ContasBancarias")
                .Property(c => c.NomeBanco)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
