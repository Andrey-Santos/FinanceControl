using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelacionamentoFatura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ContaPagarReceberId",
                table: "Faturas",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Faturas_ContaPagarReceberId",
                table: "Faturas",
                column: "ContaPagarReceberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Faturas_ContaPagarReceber_ContaPagarReceberId",
                table: "Faturas",
                column: "ContaPagarReceberId",
                principalTable: "ContaPagarReceber",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Faturas_ContaPagarReceber_ContaPagarReceberId",
                table: "Faturas");

            migrationBuilder.DropIndex(
                name: "IX_Faturas_ContaPagarReceberId",
                table: "Faturas");

            migrationBuilder.DropColumn(
                name: "ContaPagarReceberId",
                table: "Faturas");
        }
    }
}
