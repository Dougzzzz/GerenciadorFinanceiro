using GerenciadorFinanceiro.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorFinanceiro.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

     
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<ContaBancaria> ContasBancarias { get; set; }
        public DbSet<CartaoCredito> CartoesDeCredito { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

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
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                entity.Property(t => t.Cotacao)
                    .HasColumnType("decimal(18,2)");

                // Campos de texto adicionais
                entity.Property(t => t.Categoria)
                    .HasMaxLength(150);

                entity.Property(t => t.NomeCartao)
                    .HasMaxLength(150);

                entity.Property(t => t.FinalCartao)
                    .HasMaxLength(20);

                entity.Property(t => t.Parcela)
                    .HasMaxLength(50);

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
