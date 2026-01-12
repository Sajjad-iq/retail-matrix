using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_backend.Migrations
{
    /// <inheritdoc />
    public partial class RefactorPaymentToGenericEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Sales_SaleId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Sales_Customer",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Payments_Sale",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "SaleId",
                table: "Payments",
                newName: "EntityId");

            migrationBuilder.AddColumn<string>(
                name: "EntityType",
                table: "Payments",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Entity",
                table: "Payments",
                columns: new[] { "EntityId", "EntityType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_Entity",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "EntityType",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "Payments",
                newName: "SaleId");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "Sales",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Sales",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Sales",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Customer",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_Sale",
                table: "Payments",
                column: "SaleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Sales_SaleId",
                table: "Payments",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
