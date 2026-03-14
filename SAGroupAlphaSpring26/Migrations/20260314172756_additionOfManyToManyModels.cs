using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class additionOfManyToManyModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PieceSet");

            migrationBuilder.DropTable(
                name: "PieceUser");

            migrationBuilder.CreateTable(
                name: "PieceSets",
                columns: table => new
                {
                    PieceId = table.Column<int>(type: "int", nullable: false),
                    SetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PieceSets", x => new { x.PieceId, x.SetId });
                    table.ForeignKey(
                        name: "FK_PieceSets_Pieces_PieceId",
                        column: x => x.PieceId,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PieceSets_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPieces",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PieceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPieces", x => new { x.PieceId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserPieces_Pieces_PieceId",
                        column: x => x.PieceId,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPieces_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.InsertData(
                table: "Sets",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[] { 1, "Evans Beginner Pack", 0.00m });

            migrationBuilder.InsertData(
                table: "UserPieces",
                columns: new[] { "PieceId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 }
                });

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

            migrationBuilder.InsertData(
                table: "PieceSets",
                columns: new[] { "PieceId", "SetId" },
                values: new object[,]
                {
                    { 2, 1 },
                    { 5, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PieceSets_SetId",
                table: "PieceSets",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPieces_UserId",
                table: "UserPieces",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PieceSets");

            migrationBuilder.DropTable(
                name: "UserPieces");

            migrationBuilder.DeleteData(
                table: "Sets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.CreateTable(
                name: "PieceSet",
                columns: table => new
                {
                    PiecesListId = table.Column<int>(type: "int", nullable: false),
                    SetsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PieceSet", x => new { x.PiecesListId, x.SetsId });
                    table.ForeignKey(
                        name: "FK_PieceSet_Pieces_PiecesListId",
                        column: x => x.PiecesListId,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PieceSet_Sets_SetsId",
                        column: x => x.SetsId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PieceUser",
                columns: table => new
                {
                    OwnedPiecesId = table.Column<int>(type: "int", nullable: false),
                    OwnersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PieceUser", x => new { x.OwnedPiecesId, x.OwnersId });
                    table.ForeignKey(
                        name: "FK_PieceUser_Pieces_OwnedPiecesId",
                        column: x => x.OwnedPiecesId,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PieceUser_Users_OwnersId",
                        column: x => x.OwnersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 11, 18, 49, 15, 286, DateTimeKind.Local).AddTicks(2563));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 3, 11, 18, 49, 15, 286, DateTimeKind.Local).AddTicks(2567));

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

            migrationBuilder.CreateIndex(
                name: "IX_PieceSet_SetsId",
                table: "PieceSet",
                column: "SetsId");

            migrationBuilder.CreateIndex(
                name: "IX_PieceUser_OwnersId",
                table: "PieceUser",
                column: "OwnersId");
        }
    }
}
