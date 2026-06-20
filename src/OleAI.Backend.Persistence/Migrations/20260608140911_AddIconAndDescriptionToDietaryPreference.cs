using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OleAI.Backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIconAndDescriptionToDietaryPreference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Desciption",
                table: "DietaryPreferences",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "IconName",
                table: "DietaryPreferences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconName",
                table: "DietaryPreferences");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "DietaryPreferences",
                newName: "Desciption");
        }
    }
}
