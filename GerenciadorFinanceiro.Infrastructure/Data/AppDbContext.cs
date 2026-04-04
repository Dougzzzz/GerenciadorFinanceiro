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
