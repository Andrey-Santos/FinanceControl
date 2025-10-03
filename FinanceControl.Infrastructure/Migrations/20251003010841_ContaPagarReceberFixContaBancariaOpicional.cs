using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ContaPagarReceberFixContaBancariaOpicional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContaPagarReceber_ContasBancarias_ContaBancariaId",
                table: "ContaPagarReceber");

            migrationBuilder.AlterColumn<long>(
                name: "ContaBancariaId",
                table: "ContaPagarReceber",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_ContaPagarReceber_ContasBancarias_ContaBancariaId",
                table: "ContaPagarReceber",
                column: "ContaBancariaId",
                principalTable: "ContasBancarias",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContaPagarReceber_ContasBancarias_ContaBancariaId",
                table: "ContaPagarReceber");

            migrationBuilder.AlterColumn<long>(
                name: "ContaBancariaId",
                table: "ContaPagarReceber",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ContaPagarReceber_ContasBancarias_ContaBancariaId",
                table: "ContaPagarReceber",
                column: "ContaBancariaId",
                principalTable: "ContasBancarias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
