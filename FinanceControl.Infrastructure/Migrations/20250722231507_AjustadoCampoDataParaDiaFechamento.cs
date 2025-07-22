using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceControl.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustadoCampoDataParaDiaFechamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataFechamento",
                table: "Cartoes");

            migrationBuilder.AddColumn<int>(
                name: "DiaFechamento",
                table: "Cartoes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaFechamento",
                table: "Cartoes");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFechamento",
                table: "Cartoes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
