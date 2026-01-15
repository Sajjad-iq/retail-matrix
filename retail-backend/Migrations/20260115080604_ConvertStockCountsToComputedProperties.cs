using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_backend.Migrations
{
    /// <inheritdoc />
    public partial class ConvertStockCountsToComputedProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DamagedStock",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "ExpiredStock",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "GoodStock",
                table: "ProductStocks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DamagedStock",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpiredStock",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoodStock",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
