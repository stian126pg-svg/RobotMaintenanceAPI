using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RobotMaintenanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRobotOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Robots",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Robots",
                keyColumn: "Id",
                keyValue: 1,
                column: "OwnerId",
                value: "User A");

            migrationBuilder.UpdateData(
                table: "Robots",
                keyColumn: "Id",
                keyValue: 2,
                column: "OwnerId",
                value: "User B");

            migrationBuilder.UpdateData(
                table: "Robots",
                keyColumn: "Id",
                keyValue: 3,
                column: "OwnerId",
                value: "User A");

            migrationBuilder.UpdateData(
                table: "Robots",
                keyColumn: "Id",
                keyValue: 4,
                column: "OwnerId",
                value: "User B");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Robots");
        }
    }
}
