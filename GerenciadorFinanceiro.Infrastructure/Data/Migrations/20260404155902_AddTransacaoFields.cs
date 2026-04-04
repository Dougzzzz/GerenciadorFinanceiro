using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciadorFinanceiro.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTransacaoFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Transacoes",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Transacoes",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<decimal>(
                name: "Cotacao",
                table: "Transacoes",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "FinalCartao",
                table: "Transacoes",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "NomeCartao",
                table: "Transacoes",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<string>(
                name: "Parcela",
                table: "Transacoes",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: string.Empty);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "Cotacao",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "FinalCartao",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "NomeCartao",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "Parcela",
                table: "Transacoes");

            migrationBuilder.AlterColumn<decimal>(
                name: "Valor",
                table: "Transacoes",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");
        }
    }
}
