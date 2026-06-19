using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OleAI.Backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOriginalTexts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OriginalInstruction",
                table: "RecipeSteps",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OriginalDescription",
                table: "Recipes",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginalInstruction",
                table: "RecipeSteps");

            migrationBuilder.DropColumn(
                name: "OriginalDescription",
                table: "Recipes");
        }
    }
}
