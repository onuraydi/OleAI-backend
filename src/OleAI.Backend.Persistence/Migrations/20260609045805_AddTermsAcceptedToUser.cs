using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OleAI.Backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTermsAcceptedToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TermsAccepted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermsAccepted",
                table: "Users");
        }
    }
}
