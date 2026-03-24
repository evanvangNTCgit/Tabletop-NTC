using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class ArchivedColumnSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Sessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 24, 8, 42, 5, 706, DateTimeKind.Local).AddTicks(6859));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 24, 8, 42, 5, 706, DateTimeKind.Local).AddTicks(6863));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEF+0+6w5Ja8floZ6diJZx+70/JOe3otPsmpt+Xo+9WRB2rFObfLjKEJHjKls5VuhEQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDEUUoy5iMHQp7NqVSI63R//aCqhnbSa4fIfFTzqWnll7PC/0Hs0TZgarH7Gs0D2MQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Sessions");

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
    }
}
