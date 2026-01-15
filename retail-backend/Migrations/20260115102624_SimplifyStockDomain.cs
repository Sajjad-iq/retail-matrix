using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_backend.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyStockDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockBatches");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_Batch",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "BatchNumber",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "StockMovements");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReservedQuantity",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "ReservedQuantity",
                table: "ProductStocks");

            migrationBuilder.AddColumn<string>(
                name: "BatchNumber",
                table: "StockMovements",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "StockMovements",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StockBatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ProductStockId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Condition = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    InsertDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    RemainingQuantity = table.Column<int>(type: "int", nullable: false),
                    ReservedQuantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    UpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PurchaseCurrency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockBatches_ProductStocks_ProductStockId",
                        column: x => x.ProductStockId,
                        principalTable: "ProductStocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_Batch",
                table: "StockMovements",
                column: "BatchNumber");

            migrationBuilder.CreateIndex(
                name: "IX_StockBatches_BatchNumber",
                table: "StockBatches",
                column: "BatchNumber");

            migrationBuilder.CreateIndex(
                name: "IX_StockBatches_Condition",
                table: "StockBatches",
                column: "Condition");

            migrationBuilder.CreateIndex(
                name: "IX_StockBatches_Expiration",
                table: "StockBatches",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockBatches_Stock",
                table: "StockBatches",
                column: "ProductStockId");
        }
    }
}
