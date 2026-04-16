using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class AddNameToSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdated", "Name" },
                values: new object[] { new DateTime(2026, 3, 11, 18, 49, 15, 286, DateTimeKind.Local).AddTicks(2563), "Test Session" });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "LastUpdated", "Name" },
                values: new object[] { new DateTime(2026, 3, 11, 18, 49, 15, 286, DateTimeKind.Local).AddTicks(2567), "Test Session 2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEBIQPge62ArHzCYdiGbp42jCxPOoXBHq48RXx3NHhxrjtGhTsw//u2IzAD+PZN8tNw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFrPDIk6Y/nKSB5Q/d1IS4tMqHEeQ5KQB37bJRffvsI7B4YoFp3rVnRTLhjOeS54iQ==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Sessions");

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 10, 11, 12, 16, 295, DateTimeKind.Local).AddTicks(7430));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 10, 11, 12, 16, 295, DateTimeKind.Local).AddTicks(7434));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFEHJpqQt8AatBYJ0/yt0DLZVW7bXS6GQ8pfCJZiMK+im7YfbA2iSDko8SERO1jixQ==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEK6T9KyLFSstpE7m6PtSeuOeBj8CPetXljSeM0xV307aX4Kt/qxIB4BZKt1VxW/AvQ==");
        }
    }
}
