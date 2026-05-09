using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OleAI.Backend.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDislikedIngredients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllergyRestrictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllergyRestrictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DietaryPreferences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Desciption = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietaryPreferences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserDislikedIngredients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDislikedIngredients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDislikedIngredients_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDislikedIngredients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AllergyExcludedCategory",
                columns: table => new
                {
                    AllergyRestrictionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllergyExcludedCategory", x => new { x.AllergyRestrictionId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_AllergyExcludedCategory_AllergyRestrictions_AllergyRestrictionId",
                        column: x => x.AllergyRestrictionId,
                        principalTable: "AllergyRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllergyExcludedCategory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AllergyExcludedIngredient",
                columns: table => new
                {
                    AllergyRestrictionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AllergyExcludedIngredient", x => new { x.AllergyRestrictionId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_AllergyExcludedIngredient_AllergyRestrictions_AllergyRestrictionId",
                        column: x => x.AllergyRestrictionId,
                        principalTable: "AllergyRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AllergyExcludedIngredient_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAllergies",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AllergyRestrictionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAllergies", x => new { x.UserId, x.AllergyRestrictionId });
                    table.ForeignKey(
                        name: "FK_UserAllergies_AllergyRestrictions_AllergyRestrictionId",
                        column: x => x.AllergyRestrictionId,
                        principalTable: "AllergyRestrictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAllergies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DietaryPreferenceExcludedCategory",
                columns: table => new
                {
                    DietaryPreferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietaryPreferenceExcludedCategory", x => new { x.DietaryPreferenceId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_DietaryPreferenceExcludedCategory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DietaryPreferenceExcludedCategory_DietaryPreferences_DietaryPreferenceId",
                        column: x => x.DietaryPreferenceId,
                        principalTable: "DietaryPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DietaryPreferenceExcludedIngredient",
                columns: table => new
                {
                    DietaryPreferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietaryPreferenceExcludedIngredient", x => new { x.DietaryPreferenceId, x.IngredientId });
                    table.ForeignKey(
                        name: "FK_DietaryPreferenceExcludedIngredient_DietaryPreferences_DietaryPreferenceId",
                        column: x => x.DietaryPreferenceId,
                        principalTable: "DietaryPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DietaryPreferenceExcludedIngredient_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDietaryPreferences",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DietaryPreferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDietaryPreferences", x => new { x.UserId, x.DietaryPreferenceId });
                    table.ForeignKey(
                        name: "FK_UserDietaryPreferences_DietaryPreferences_DietaryPreferenceId",
                        column: x => x.DietaryPreferenceId,
                        principalTable: "DietaryPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserDietaryPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllergyExcludedCategory_CategoryId",
                table: "AllergyExcludedCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AllergyExcludedIngredient_IngredientId",
                table: "AllergyExcludedIngredient",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_AllergyRestrictions_Name",
                table: "AllergyRestrictions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DietaryPreferenceExcludedCategory_CategoryId",
                table: "DietaryPreferenceExcludedCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DietaryPreferenceExcludedIngredient_IngredientId",
                table: "DietaryPreferenceExcludedIngredient",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_DietaryPreferences_Name",
                table: "DietaryPreferences",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAllergies_AllergyRestrictionId",
                table: "UserAllergies",
                column: "AllergyRestrictionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDietaryPreferences_DietaryPreferenceId",
                table: "UserDietaryPreferences",
                column: "DietaryPreferenceId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDislikedIngredients_IngredientId",
                table: "UserDislikedIngredients",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDislikedIngredients_UserId_IngredientId",
                table: "UserDislikedIngredients",
                columns: new[] { "UserId", "IngredientId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllergyExcludedCategory");

            migrationBuilder.DropTable(
                name: "AllergyExcludedIngredient");

            migrationBuilder.DropTable(
                name: "DietaryPreferenceExcludedCategory");

            migrationBuilder.DropTable(
                name: "DietaryPreferenceExcludedIngredient");

            migrationBuilder.DropTable(
                name: "UserAllergies");

            migrationBuilder.DropTable(
                name: "UserDietaryPreferences");

            migrationBuilder.DropTable(
                name: "UserDislikedIngredients");

            migrationBuilder.DropTable(
                name: "AllergyRestrictions");

            migrationBuilder.DropTable(
                name: "DietaryPreferences");
        }
    }
}
