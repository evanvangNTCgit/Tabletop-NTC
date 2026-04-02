using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class AdditionOfCartItems2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 2, 12, 35, 58, 27, DateTimeKind.Local).AddTicks(6634));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 2, 12, 35, 58, 27, DateTimeKind.Local).AddTicks(6638));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFoxFEJ+GOAvTcNaIY3evf7abKMUdg/Kom2gBi06zN7fcRTNjvEZNQEVpyIk6Ag8Qw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJJjbDNQjjVcEm72d7tvKYB52cYuIM58UIZp9jzFTGHqgJa3Wr+TtBMUFbj/NNi9fQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 2, 11, 36, 31, 323, DateTimeKind.Local).AddTicks(2616));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 2, 11, 36, 31, 323, DateTimeKind.Local).AddTicks(2621));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGjJ6My1lFgOF2yh1Im2QLr/L7BVyAJ5ibikUjLaRcRmWpB/hw1iu7x7OY3fTFd+6Q==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEN3DueMUMcHi8DwiS9Bvi6YAV0rxy6U28QkcCzVLbb1jV4QVlud+4q+FK1KPck6Vpg==");
        }
    }
}
