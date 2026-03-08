using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class seedDataMovedToDataContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PieceTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Player" },
                    { 2, "Map" },
                    { 3, "Structure" },
                    { 4, "Object" },
                    { 5, "Goblin" },
                    { 6, "Orc" },
                    { 7, "Shop" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Username" },
                values: new object[] { 1, "local@demo.com", "LocalDM" });

            migrationBuilder.InsertData(
                table: "Pieces",
                columns: new[] { "Id", "Description", "ImagePath", "Name", "PieceTypeID", "Price" },
                values: new object[,]
                {
                    { 1, "No description provided", "/images/testMap.png", "Default Dungeon", 2, 0.00m },
                    { 2, "No description provided", "/images/Cleric.png", "Cleric", 1, 0.00m },
                    { 3, "No description provided", "/images/GoblinChief.png", "Goblin Chief", 5, 0.00m },
                    { 4, "No description provided", "/images/chest.png", "Basic Chest", 4, 0.00m },
                    { 5, "No description provided", "/images/bardTest.png", "Bard", 1, 0.00m }
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "LastUpdated", "Notes", "UserId" },
                values: new object[] { 1, new DateTime(2026, 3, 8, 16, 36, 25, 672, DateTimeKind.Local).AddTicks(3766), "Local Test Session", 1 });

            migrationBuilder.InsertData(
                table: "Tokens",
                columns: new[] { "Id", "Name", "PieceID", "SessionId", "Visibility", "X", "Y", "ZIndex" },
                values: new object[,]
                {
                    { 1, "Default Dungeon", 2, 1, true, 0m, 0m, 0m },
                    { 2, "Cleric", 1, 1, true, 50m, 15m, 3m },
                    { 3, "Goblin Chief", 5, 1, true, 50m, 5m, 1m },
                    { 4, "Basic Chest", 4, 1, false, 50m, 10m, 2m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
