using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OleAI.Backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIngredientAliasIdToUserIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UK_UserIngredients_UserId_IngredientId",
                table: "UserIngredients");

            migrationBuilder.AddColumn<Guid>(
                name: "IngredientAliasId",
                table: "UserIngredients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserIngredients_IngredientAliasId",
                table: "UserIngredients",
                column: "IngredientAliasId");

            migrationBuilder.CreateIndex(
                name: "UK_UserIngredients_UserId_IngredientId_AliasId",
                table: "UserIngredients",
                columns: new[] { "UserId", "IngredientId", "IngredientAliasId" },
                unique: true,
                filter: "[IngredientId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UserIngredients_IngredientAliases_IngredientAliasId",
                table: "UserIngredients",
                column: "IngredientAliasId",
                principalTable: "IngredientAliases",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserIngredients_IngredientAliases_IngredientAliasId",
                table: "UserIngredients");

            migrationBuilder.DropIndex(
                name: "IX_UserIngredients_IngredientAliasId",
                table: "UserIngredients");

            migrationBuilder.DropIndex(
                name: "UK_UserIngredients_UserId_IngredientId_AliasId",
                table: "UserIngredients");

            migrationBuilder.DropColumn(
                name: "IngredientAliasId",
                table: "UserIngredients");

            migrationBuilder.CreateIndex(
                name: "UK_UserIngredients_UserId_IngredientId",
                table: "UserIngredients",
                columns: new[] { "UserId", "IngredientId" },
                unique: true,
                filter: "[IngredientId] IS NOT NULL");
        }
    }
}
