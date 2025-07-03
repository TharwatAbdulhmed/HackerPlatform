using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HackerPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeModuleHaveMultipleVulnerabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Modules_Vulnerabilities_VulnerabilityId",
                table: "Modules");

            migrationBuilder.DropIndex(
                name: "IX_Modules_VulnerabilityId",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "VulnerabilityId",
                table: "Modules");

            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "Vulnerabilities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vulnerabilities_ModuleId",
                table: "Vulnerabilities",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vulnerabilities_Modules_ModuleId",
                table: "Vulnerabilities",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vulnerabilities_Modules_ModuleId",
                table: "Vulnerabilities");

            migrationBuilder.DropIndex(
                name: "IX_Vulnerabilities_ModuleId",
                table: "Vulnerabilities");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Vulnerabilities");

            migrationBuilder.AddColumn<int>(
                name: "VulnerabilityId",
                table: "Modules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Modules_VulnerabilityId",
                table: "Modules",
                column: "VulnerabilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Modules_Vulnerabilities_VulnerabilityId",
                table: "Modules",
                column: "VulnerabilityId",
                principalTable: "Vulnerabilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
