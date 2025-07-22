using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCartaoContaBancariaRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cartoes_Bancos_BancoId",
                table: "Cartoes");

            migrationBuilder.AlterColumn<long>(
                name: "BancoId",
                table: "Cartoes",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "ContaBancariaId",
                table: "Cartoes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Cartoes_ContaBancariaId",
                table: "Cartoes",
                column: "ContaBancariaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cartoes_Bancos_BancoId",
                table: "Cartoes",
                column: "BancoId",
                principalTable: "Bancos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cartoes_ContasBancarias_ContaBancariaId",
                table: "Cartoes",
                column: "ContaBancariaId",
                principalTable: "ContasBancarias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cartoes_Bancos_BancoId",
                table: "Cartoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Cartoes_ContasBancarias_ContaBancariaId",
                table: "Cartoes");

            migrationBuilder.DropIndex(
                name: "IX_Cartoes_ContaBancariaId",
                table: "Cartoes");

            migrationBuilder.DropColumn(
                name: "ContaBancariaId",
                table: "Cartoes");

            migrationBuilder.AlterColumn<long>(
                name: "BancoId",
                table: "Cartoes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cartoes_Bancos_BancoId",
                table: "Cartoes",
                column: "BancoId",
                principalTable: "Bancos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
