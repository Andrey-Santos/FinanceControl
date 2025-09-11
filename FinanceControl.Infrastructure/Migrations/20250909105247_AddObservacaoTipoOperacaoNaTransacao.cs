using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddObservacaoTipoOperacaoNaTransacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Observacao",
                table: "Transacoes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoOperacao",
                table: "Transacoes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Observacao",
                table: "Transacoes");

            migrationBuilder.DropColumn(
                name: "TipoOperacao",
                table: "Transacoes");
        }
    }
}
