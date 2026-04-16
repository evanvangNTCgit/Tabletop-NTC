using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class AddPieceSetsDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 14, 12, 58, 26, 289, DateTimeKind.Local).AddTicks(3271));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 14, 12, 58, 26, 289, DateTimeKind.Local).AddTicks(3275));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEBIqhBMNHNehv1yW8fXhwC5zzpA6BgmLBbA5GTsHXOpn+WkdIo+gwin7fsEGUfUNDw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIK8C4A7QXgEHz4IC2MPcHEiSOv0EfQHwZ0XQbwqjQdVhUGJY9WQkZW47YRM/Buz1g==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 14, 12, 27, 56, 417, DateTimeKind.Local).AddTicks(9750));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 14, 12, 27, 56, 417, DateTimeKind.Local).AddTicks(9754));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEP2QYspcgDrGhpAHTJLUBGZBosTnzFkCHEo+iLOB5z8x27T/kTJpPjjlQfkJHD2Zug==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEE1pEf8L7Sx8abRWrXGlWZEUXR+irLNoVb6Vv4F9MP8bLSGVdvBBs/lQMd2Sm/vTgg==");
        }
    }
}
