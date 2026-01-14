using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReceivedDateFromStockBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceivedDate",
                table: "StockBatches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivedDate",
                table: "StockBatches",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
