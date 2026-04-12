using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciadorFinanceiro.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProvedorToContasECartoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Provedor",
                table: "ContasBancarias",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Provedor",
                table: "CartoesDeCredito",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_CartaoCreditoId",
                table: "Transacoes",
                column: "CartaoCreditoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_CategoriaId",
                table: "Transacoes",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ContaBancariaId",
                table: "Transacoes",
                column: "ContaBancariaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_CartoesDeCredito_CartaoCreditoId",
                table: "Transacoes",
                column: "CartaoCreditoId",
                principalTable: "CartoesDeCredito",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_Categorias_CategoriaId",
                table: "Transacoes",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_ContasBancarias_ContaBancariaId",
                table: "Transacoes",
                column: "ContaBancariaId",
                principalTable: "ContasBancarias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_CartoesDeCredito_CartaoCreditoId",
                table: "Transacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_Categorias_CategoriaId",
                table: "Transacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_ContasBancarias_ContaBancariaId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_CartaoCreditoId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_CategoriaId",
                table: "Transacoes");

            migrationBuilder.DropIndex(
                name: "IX_Transacoes_ContaBancariaId",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "Provedor",
                table: "ContasBancarias");

            migrationBuilder.DropColumn(
                name: "Provedor",
                table: "CartoesDeCredito");
        }
    }
}
