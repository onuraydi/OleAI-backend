using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OleAI.Backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SupportCustomShoppingListItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "IngredientId",
                table: "ShoppingListItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "IngredientRequestId",
                table: "ShoppingListItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListItems_IngredientRequestId",
                table: "ShoppingListItems",
                column: "IngredientRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeIngredients_IngredientRequestId",
                table: "RecipeIngredients",
                column: "IngredientRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecipeIngredients_IngredientRequests_IngredientRequestId",
                table: "RecipeIngredients",
                column: "IngredientRequestId",
                principalTable: "IngredientRequests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingListItems_IngredientRequests_IngredientRequestId",
                table: "ShoppingListItems",
                column: "IngredientRequestId",
                principalTable: "IngredientRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecipeIngredients_IngredientRequests_IngredientRequestId",
                table: "RecipeIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingListItems_IngredientRequests_IngredientRequestId",
                table: "ShoppingListItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingListItems_IngredientRequestId",
                table: "ShoppingListItems");

            migrationBuilder.DropIndex(
                name: "IX_RecipeIngredients_IngredientRequestId",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "IngredientRequestId",
                table: "ShoppingListItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "IngredientId",
                table: "ShoppingListItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
