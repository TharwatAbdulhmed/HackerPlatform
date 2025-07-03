using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HackerPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCleanStart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Modules_ModuleId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "Choices");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Achievements");

            migrationBuilder.RenameColumn(
                name: "ModuleId",
                table: "Questions",
                newName: "VulnerabilityId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_ModuleId",
                table: "Questions",
                newName: "IX_Questions_VulnerabilityId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Vulnerabilities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "AnswerOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerOptions_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerOptions_QuestionId",
                table: "AnswerOptions",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Vulnerabilities_VulnerabilityId",
                table: "Questions",
                column: "VulnerabilityId",
                principalTable: "Vulnerabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Vulnerabilities_VulnerabilityId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "AnswerOptions");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Vulnerabilities");

            migrationBuilder.RenameColumn(
                name: "VulnerabilityId",
                table: "Questions",
                newName: "ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_VulnerabilityId",
                table: "Questions",
                newName: "IX_Questions_ModuleId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Achievements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Choices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Choices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Choices_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Choices_QuestionId",
                table: "Choices",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Modules_ModuleId",
                table: "Questions",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
