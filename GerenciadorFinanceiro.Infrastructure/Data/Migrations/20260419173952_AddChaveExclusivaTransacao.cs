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
                nullable: false,
                defaultValue: string.Empty);

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
