using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLocationIdFromProductStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_Location",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_Location",
                table: "ProductStocks",
                column: "LocationId");
        }
    }
}
