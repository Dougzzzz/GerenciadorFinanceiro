using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GerenciadorFinanceiro.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMetasGastos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetasGastos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoriaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ValorLimite = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Mes = table.Column<int>(type: "integer", nullable: true),
                    Ano = table.Column<int>(type: "integer", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetasGastos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetasGastos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetasGastos_CategoriaId",
                table: "MetasGastos",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(
                name: "MetasGastos");
    }
}
