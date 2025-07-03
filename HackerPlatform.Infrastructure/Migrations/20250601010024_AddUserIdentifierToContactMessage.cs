using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HackerPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdentifierToContactMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserIdentifier",
                table: "ContactMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserIdentifier",
                table: "ContactMessages");
        }
    }
}
