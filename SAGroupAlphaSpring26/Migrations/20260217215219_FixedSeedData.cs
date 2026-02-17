using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class FixedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PieceTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Enemy" },
                    { 2, "Map" }
                });

            migrationBuilder.InsertData(
                table: "Sets",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[] { 1, "Base Set", 0.00m });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Username" },
                values: new object[] { 1, new DateTime(2026, 2, 17, 15, 52, 19, 283, DateTimeKind.Local).AddTicks(5084), "tjackson@students.ntc.edu", "Fred" });

            migrationBuilder.InsertData(
                table: "Pieces",
                columns: new[] { "Id", "Description", "ImagePath", "Name", "PieceTypeID", "Price", "SetID" },
                values: new object[,]
                {
                    { 1, "Default Description", "/images/default.png", "Default Dungeon", 2, 0.00m, 1 },
                    { 2, "Default Description", "/images/goblin.png", "Goblin", 1, 0.00m, 1 },
                    { 3, "Default Description", "/images/hero-knight.png", "Orc", 1, 0.00m, 1 }
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "LastUpdated", "Notes", "UserId" },
                values: new object[] { 1, new DateTime(2026, 2, 17, 15, 52, 19, 283, DateTimeKind.Local).AddTicks(5154), "Initial Test Session", 1 });

            migrationBuilder.InsertData(
                table: "Tokens",
                columns: new[] { "Id", "IsVisible", "Name", "PieceID", "SessionID", "X", "Y", "zIndex" },
                values: new object[,]
                {
                    { 1, true, "Active Map", 1, 1, 0.0, 0.0, 0 },
                    { 2, true, "Goblin", 2, 1, 50.0, 5.0, 1 },
                    { 3, true, "Knight", 3, 1, 50.0, 10.0, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                keyValue: 3);

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
                table: "Sets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
