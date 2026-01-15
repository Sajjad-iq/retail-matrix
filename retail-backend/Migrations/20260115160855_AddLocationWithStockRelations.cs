using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationWithStockRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_Packaging_Organization",
                table: "ProductStocks");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "ProductStocks",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrganizationId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    ParentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    InsertDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Locations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_Location",
                table: "StockMovements",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_Location",
                table: "ProductStocks",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_Packaging_Organization_Location",
                table: "ProductStocks",
                columns: new[] { "ProductPackagingId", "OrganizationId", "LocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Code_Organization",
                table: "Locations",
                columns: new[] { "Code", "OrganizationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Organization",
                table: "Locations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Parent",
                table: "Locations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Type",
                table: "Locations",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStocks_Locations_LocationId",
                table: "ProductStocks",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Locations_LocationId",
                table: "StockMovements",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductStocks_Locations_LocationId",
                table: "ProductStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Locations_LocationId",
                table: "StockMovements");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_Location",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_Location",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_Packaging_Organization_Location",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "ProductStocks");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_Packaging_Organization",
                table: "ProductStocks",
                columns: new[] { "ProductPackagingId", "OrganizationId" },
                unique: true);
        }
    }
}
