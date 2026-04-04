using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OleAI.Backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorUserStatusToEmailVerifiedAndIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Users",
                newName: "IsEmailVerified");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "IsEmailVerified",
                table: "Users",
                newName: "Status");
        }
    }
}
