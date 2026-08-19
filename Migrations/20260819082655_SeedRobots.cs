using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RobotMaintenanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedRobots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Robots",
                columns: new[] { "Id", "LastMaintenance", "Model", "Name", "NextMaintenance", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "XR-7", "Atlas", new DateTime(2026, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Operational" },
                    { 2, new DateTime(2026, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "MK-II", "Hammer", new DateTime(2026, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "NeedsMaintenance" },
                    { 3, new DateTime(2026, 7, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "RX-12", "Bishop", new DateTime(2026, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Operational" },
                    { 4, new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "MK-I", "Rustbucket", null, "OutOfService" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Robots",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Robots",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Robots",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Robots",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
