using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class AdditionOfCartItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PieceId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Pieces_PieceId",
                        column: x => x.PieceId,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Pieces",
                columns: new[] { "Id", "Description", "ImagePath", "IsArchived", "Name", "PieceTypeID", "Price" },
                values: new object[] { 6, "No description provided", "/images/BetaMap1.png", false, "Beta Dungeon", 2, 5.00m });

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
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "PieceID" },
                values: new object[] { "Beta Dungeon", 6 });

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

            migrationBuilder.InsertData(
                table: "UserPieces",
                columns: new[] { "PieceId", "UserId" },
                values: new object[] { 6, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_PieceId",
                table: "CartItems",
                column: "PieceId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DeleteData(
                table: "UserPieces",
                keyColumns: new[] { "PieceId", "UserId" },
                keyValues: new object[] { 6, 1 });

            migrationBuilder.DeleteData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 24, 9, 43, 9, 756, DateTimeKind.Local).AddTicks(7977));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 24, 9, 43, 9, 756, DateTimeKind.Local).AddTicks(7981));

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "PieceID" },
                values: new object[] { "Default Dungeon", 1 });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEGg97jqBmuqi0yFSgNAa/jBzR/yjb+muPVBN4aH3AEsMoofex1IIoqZU0uOfGrhj5Q==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEL/669zvfRAYfPW3x7rIex/kN7IX0BsdUF4ylsJkTzXNDKiU4Wh8UiCVSxtjWdoXCQ==");
        }
    }
}
