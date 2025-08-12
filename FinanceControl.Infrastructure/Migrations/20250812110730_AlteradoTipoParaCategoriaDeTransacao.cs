using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlteradoTipoParaCategoriaDeTransacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_TiposTransacao_TipoId",
                table: "Transacoes");

            migrationBuilder.DropTable(
                name: "TiposTransacao");

            migrationBuilder.RenameColumn(
                name: "TipoId",
                table: "Transacoes",
                newName: "CategoriaId");

            migrationBuilder.RenameIndex(
                name: "IX_Transacoes_TipoId",
                table: "Transacoes",
                newName: "IX_Transacoes_CategoriaId");

            migrationBuilder.CreateTable(
                name: "CategoriasTransacao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasTransacao", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_CategoriasTransacao_CategoriaId",
                table: "Transacoes",
                column: "CategoriaId",
                principalTable: "CategoriasTransacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacoes_CategoriasTransacao_CategoriaId",
                table: "Transacoes");

            migrationBuilder.DropTable(
                name: "CategoriasTransacao");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "Transacoes",
                newName: "TipoId");

            migrationBuilder.RenameIndex(
                name: "IX_Transacoes_CategoriaId",
                table: "Transacoes",
                newName: "IX_Transacoes_TipoId");

            migrationBuilder.CreateTable(
                name: "TiposTransacao",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataAlteracao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposTransacao", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Transacoes_TiposTransacao_TipoId",
                table: "Transacoes",
                column: "TipoId",
                principalTable: "TiposTransacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
