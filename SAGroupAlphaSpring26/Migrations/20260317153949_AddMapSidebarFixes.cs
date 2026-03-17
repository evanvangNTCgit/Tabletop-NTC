using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class AddMapSidebarFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 17, 10, 39, 49, 476, DateTimeKind.Local).AddTicks(9858));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 17, 10, 39, 49, 476, DateTimeKind.Local).AddTicks(9862));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAXcpfnXJMOyO7Pu7YqJ6wMpYGyFaW3qqW6LqDfACzOlBQLlY0PBSmEojsVZls6rLA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPC9tZgFoHsUlkAiWRgJQpHn9lRbpi9gjWblc5bKRLTeKIVCeAAbVixPG/W7TtKAew==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 17, 10, 39, 33, 216, DateTimeKind.Local).AddTicks(103));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 17, 10, 39, 33, 216, DateTimeKind.Local).AddTicks(107));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHjvtwC03+v+gLa0ZUwfZuK8UwK8ZWBRrHA4OWivceY/G0hrSvDLg6GJdRjxusCZJA==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKD1Lo4KWIpCbWkrxHNXv1GQxH0xHPZQYA0njf7rEzknodYwUZUgWdmSo2LeuZTBig==");
        }
    }
}
