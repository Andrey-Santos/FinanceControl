using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ContaPagarReceberAddCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CategoriaId",
                table: "ContaPagarReceber",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContaPagarReceber_CategoriaId",
                table: "ContaPagarReceber",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContaPagarReceber_CategoriasTransacao_CategoriaId",
                table: "ContaPagarReceber",
                column: "CategoriaId",
                principalTable: "CategoriasTransacao",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContaPagarReceber_CategoriasTransacao_CategoriaId",
                table: "ContaPagarReceber");

            migrationBuilder.DropIndex(
                name: "IX_ContaPagarReceber_CategoriaId",
                table: "ContaPagarReceber");

            migrationBuilder.DropColumn(
                name: "CategoriaId",
                table: "ContaPagarReceber");
        }
    }
}
