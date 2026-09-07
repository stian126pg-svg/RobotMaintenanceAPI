using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RobotMaintenanceAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Robots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:IdentitySequenceOptions", "'5', '1', '', '', 'False', '1'")
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Model = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    LastMaintenance = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NextMaintenance = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Robots", x => x.Id);
                });

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
            migrationBuilder.DropTable(
                name: "Robots");
        }
    }
}
