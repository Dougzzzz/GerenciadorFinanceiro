using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciadorFinanceiro.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddChaveExclusivaTransacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChaveExclusiva",
                table: "Transacoes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            // Popula os dados existentes com o hash compatível com a lógica da entidade C#
            if (migrationBuilder.ActiveProvider == "Npgsql.EntityFrameworkCore.PostgreSQL")
            {
                migrationBuilder.Sql(@"
                    UPDATE ""Transacoes"" 
                    SET ""ChaveExclusiva"" = UPPER(encode(sha256(
                        (to_char(""Data"", 'YYYYMMDD') || '-' || 
                         UPPER(TRIM(""Descricao"")) || '-' || 
                         to_char(""Valor"", 'FM999999990.00') || '-' || 
                         COALESCE(""ContaBancariaId""::text, '') || '-' || 
                         COALESCE(""CartaoCreditoId""::text, '') || '-' || 
                         COALESCE(""Parcela"", ''))::bytea
                    ), 'hex'));
                ");
            }
            else
            {
                // Para SQLite (testes), apenas preenchemos com um valor temporário para evitar erro de índice único
                // Já que os testes costumam criar a base do zero, isto raramente terá impacto real.
                migrationBuilder.Sql("UPDATE \"Transacoes\" SET \"ChaveExclusiva\" = \"Id\";");
            }

            // Define como obrigatório após popular
            migrationBuilder.AlterColumn<string>(
                name: "ChaveExclusiva",
                table: "Transacoes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ChaveExclusiva",
                table: "Transacoes",
                column: "ChaveExclusiva",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transacoes_ChaveExclusiva",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "ChaveExclusiva",
                table: "Transacoes");
        }
    }
}
