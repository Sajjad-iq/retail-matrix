using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddBatchLevelReservations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRestockDate",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "ReservedStock",
                table: "ProductStocks");

            migrationBuilder.AddColumn<int>(
                name: "ReservedQuantity",
                table: "StockBatches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservedQuantity",
                table: "StockBatches");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRestockDate",
                table: "ProductStocks",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReservedStock",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
