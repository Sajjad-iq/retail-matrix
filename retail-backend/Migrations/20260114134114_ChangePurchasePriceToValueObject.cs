using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_backend.Migrations
{
    /// <inheritdoc />
    public partial class ChangePurchasePriceToValueObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CostPrice",
                table: "StockBatches",
                newName: "PurchasePrice");

            migrationBuilder.AddColumn<string>(
                name: "PurchaseCurrency",
                table: "StockBatches",
                type: "varchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseCurrency",
                table: "StockBatches");

            migrationBuilder.RenameColumn(
                name: "PurchasePrice",
                table: "StockBatches",
                newName: "CostPrice");
        }
    }
}
