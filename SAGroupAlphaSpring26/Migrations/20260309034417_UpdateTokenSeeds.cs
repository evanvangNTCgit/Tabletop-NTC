using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTokenSeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 8, 22, 44, 16, 937, DateTimeKind.Local).AddTicks(8512));

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "LastUpdated", "Notes", "UserId" },
                values: new object[] { 2, new DateTime(2026, 3, 8, 22, 44, 16, 937, DateTimeKind.Local).AddTicks(8515), "Local Test Session 2", 1 });

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 1,
                column: "PieceID",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 2,
                column: "PieceID",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 3,
                column: "PieceID",
                value: 3);

            migrationBuilder.InsertData(
                table: "Tokens",
                columns: new[] { "Id", "Name", "PieceID", "SessionId", "Visibility", "X", "Y", "ZIndex" },
                values: new object[,]
                {
                    { 5, "Default Dungeon", 1, 2, true, 0m, 0m, 0m },
                    { 6, "Cleric", 2, 2, true, 50m, 15m, 3m },
                    { 7, "Cleric", 2, 2, true, 50m, 5m, 1m },
                    { 8, "Cleric", 2, 2, true, 50m, 10m, 2m },
                    { 9, "Goblin Chief", 3, 2, true, 50m, 5m, 1m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 8, 16, 36, 25, 672, DateTimeKind.Local).AddTicks(3766));

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 1,
                column: "PieceID",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 2,
                column: "PieceID",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 3,
                column: "PieceID",
                value: 5);
        }
    }
}
