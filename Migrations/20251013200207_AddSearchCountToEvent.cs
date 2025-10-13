using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Programming_7312_Part_1.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchCountToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SearchCount",
                table: "Events",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchCount",
                table: "Events");
        }
    }
}
