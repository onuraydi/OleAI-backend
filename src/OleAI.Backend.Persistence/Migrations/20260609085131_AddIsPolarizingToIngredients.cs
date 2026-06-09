using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OleAI.Backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPolarizingToIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPolarizing",
                table: "Ingredients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPolarizing",
                table: "Ingredients");
        }
    }
}
