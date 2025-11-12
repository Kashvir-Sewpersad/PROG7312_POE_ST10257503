using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Programming_7312_Part_1.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminResponseFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminComments",
                table: "Issues",
                type: "TEXT",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminResponse",
                table: "Issues",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseDate",
                table: "Issues",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminComments",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "AdminResponse",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "ResponseDate",
                table: "Issues");
        }
    }
}
