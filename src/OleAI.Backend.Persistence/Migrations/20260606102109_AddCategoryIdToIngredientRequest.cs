using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OleAI.Backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIdToIngredientRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "IngredientRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientRequests_CategoryId",
                table: "IngredientRequests",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_IngredientRequests_Categories_CategoryId",
                table: "IngredientRequests",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IngredientRequests_Categories_CategoryId",
                table: "IngredientRequests");

            migrationBuilder.DropIndex(
                name: "IX_IngredientRequests_CategoryId",
                table: "IngredientRequests");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "IngredientRequests");
        }
    }
}
