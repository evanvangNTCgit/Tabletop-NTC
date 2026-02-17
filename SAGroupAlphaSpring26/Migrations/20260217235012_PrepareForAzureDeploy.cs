using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class PrepareForAzureDeploy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Player");

            migrationBuilder.InsertData(
                table: "PieceTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 3, "Structure" },
                    { 4, "Object" },
                    { 5, "Goblin" },
                    { 6, "Orc" },
                    { 7, "Shop" }
                });

            migrationBuilder.UpdateData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImagePath", "Name", "PieceTypeID" },
                values: new object[] { "/images/Cleric.png", "Cleric", 1 });

            migrationBuilder.UpdateData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImagePath", "Name", "PieceTypeID" },
                values: new object[] { "/images/testMap.png", "Default Dungeon", 2 });

            migrationBuilder.UpdateData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImagePath", "Name" },
                values: new object[] { "/images/GoblinChief.png", "Goblin Chief" });

            migrationBuilder.InsertData(
                table: "Pieces",
                columns: new[] { "Id", "Description", "ImagePath", "Name", "PieceTypeID", "Price", "SetID" },
                values: new object[] { 4, "Default Description", "/images/chest.png", "Basic Chest", 1, 0.00m, 1 });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdated", "Notes" },
                values: new object[] { new DateTime(2026, 2, 17, 17, 50, 12, 338, DateTimeKind.Local).AddTicks(998), "Production Test Session" });

            migrationBuilder.InsertData(
                table: "Sets",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[] { 2, "Expansion 1", 4.99m });

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "PieceID" },
                values: new object[] { "Default Dungeon", 2 });

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "PieceID" },
                values: new object[] { "Goblin Chief", 3 });

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsVisible", "Name", "PieceID" },
                values: new object[] { false, "Basic Chest", 4 });

            migrationBuilder.InsertData(
                table: "Tokens",
                columns: new[] { "Id", "IsVisible", "Name", "PieceID", "SessionID", "X", "Y", "zIndex" },
                values: new object[,]
                {
                    { 4, true, "Cleric", 1, 1, 50.0, 15.0, 3 },
                    { 5, true, "Cleric", 1, 1, 50.0, 20.0, 4 }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Username" },
                values: new object[] { new DateTime(2026, 2, 17, 17, 50, 12, 338, DateTimeKind.Local).AddTicks(950), "Tristan" });
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
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 5);

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
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Sets",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Enemy");

            migrationBuilder.UpdateData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImagePath", "Name", "PieceTypeID" },
                values: new object[] { "/images/default.png", "Default Dungeon", 2 });

            migrationBuilder.UpdateData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImagePath", "Name", "PieceTypeID" },
                values: new object[] { "/images/goblin.png", "Goblin", 1 });

            migrationBuilder.UpdateData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImagePath", "Name" },
                values: new object[] { "/images/hero-knight.png", "Orc" });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "LastUpdated", "Notes" },
                values: new object[] { new DateTime(2026, 2, 17, 16, 23, 44, 913, DateTimeKind.Local).AddTicks(1827), "Initial Test Session" });

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Name", "PieceID" },
                values: new object[] { "Active Map", 1 });

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Name", "PieceID" },
                values: new object[] { "Goblin", 2 });

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IsVisible", "Name", "PieceID" },
                values: new object[] { true, "Knight", 3 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Username" },
                values: new object[] { new DateTime(2026, 2, 17, 16, 23, 44, 913, DateTimeKind.Local).AddTicks(1787), "Fred" });
        }
    }
}
