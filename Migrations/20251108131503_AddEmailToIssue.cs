using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Programming_7312_Part_1.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToIssue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Issues",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Issues");
        }
    }
}
