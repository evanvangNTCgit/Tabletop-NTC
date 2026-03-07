using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class MajorModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pieces_Sets_SetID",
                table: "Pieces");

            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Sessions_SessionID",
                table: "Tokens");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "Marketplaces");

            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "Receipts");

            migrationBuilder.DropIndex(
                name: "IX_Pieces_SetID",
                table: "Pieces");

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
                table: "Sets",
                keyColumn: "Id",
                keyValue: 2);

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
                table: "Tokens",
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
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Pieces",
                keyColumn: "Id",
                keyValue: 4);

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

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SetID",
                table: "Pieces");

            migrationBuilder.RenameColumn(
                name: "zIndex",
                table: "Tokens",
                newName: "ZIndex");

            migrationBuilder.RenameColumn(
                name: "SessionID",
                table: "Tokens",
                newName: "SessionId");

            migrationBuilder.RenameColumn(
                name: "IsVisible",
                table: "Tokens",
                newName: "Visibility");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_SessionID",
                table: "Tokens",
                newName: "IX_Tokens_SessionId");

            migrationBuilder.AlterColumn<decimal>(
                name: "ZIndex",
                table: "Tokens",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Y",
                table: "Tokens",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "X",
                table: "Tokens",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Pieces",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SaleID = table.Column<int>(type: "int", nullable: false),
                    PieceID = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleLines_Pieces_PieceID",
                        column: x => x.PieceID,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleLines_Sales_SaleID",
                        column: x => x.SaleID,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PieceSet_SetsId",
                table: "PieceSet",
                column: "SetsId");

            migrationBuilder.CreateIndex(
                name: "IX_PieceUser_OwnersId",
                table: "PieceUser",
                column: "OwnersId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleLines_PieceID",
                table: "SaleLines",
                column: "PieceID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleLines_SaleID",
                table: "SaleLines",
                column: "SaleID");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_UserID",
                table: "Sales",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Sessions_SessionId",
                table: "Tokens",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Sessions_SessionId",
                table: "Tokens");

            migrationBuilder.DropTable(
                name: "PieceSet");

            migrationBuilder.DropTable(
                name: "PieceUser");

            migrationBuilder.DropTable(
                name: "SaleLines");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.RenameColumn(
                name: "ZIndex",
                table: "Tokens",
                newName: "zIndex");

            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "Tokens",
                newName: "SessionID");

            migrationBuilder.RenameColumn(
                name: "Visibility",
                table: "Tokens",
                newName: "IsVisible");

            migrationBuilder.RenameIndex(
                name: "IX_Tokens_SessionId",
                table: "Tokens",
                newName: "IX_Tokens_SessionID");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "zIndex",
                table: "Tokens",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "Y",
                table: "Tokens",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<double>(
                name: "X",
                table: "Tokens",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Pieces",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SetID",
                table: "Pieces",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Marketplaces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SetId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marketplaces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PieceId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Purchases_Pieces_PieceId",
                        column: x => x.PieceId,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Purchases_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Receipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PieceID = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PurchaseID = table.Column<int>(type: "int", nullable: false),
                    SetID = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Receipts", x => x.Id);
                });

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
                table: "Sets",
                columns: new[] { "Id", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Base Set", 0.00m },
                    { 2, "Expansion 1", 4.99m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "Username" },
                values: new object[] { 1, new DateTime(2026, 2, 17, 17, 50, 12, 338, DateTimeKind.Local).AddTicks(950), "tjackson@students.ntc.edu", "Tristan" });

            migrationBuilder.InsertData(
                table: "Pieces",
                columns: new[] { "Id", "Description", "ImagePath", "Name", "PieceTypeID", "Price", "SetID" },
                values: new object[,]
                {
                    { 1, "Default Description", "/images/Cleric.png", "Cleric", 1, 0.00m, 1 },
                    { 2, "Default Description", "/images/testMap.png", "Default Dungeon", 2, 0.00m, 1 },
                    { 3, "Default Description", "/images/GoblinChief.png", "Goblin Chief", 1, 0.00m, 1 },
                    { 4, "Default Description", "/images/chest.png", "Basic Chest", 1, 0.00m, 1 }
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "LastUpdated", "Notes", "UserId" },
                values: new object[] { 1, new DateTime(2026, 2, 17, 17, 50, 12, 338, DateTimeKind.Local).AddTicks(998), "Production Test Session", 1 });

            migrationBuilder.InsertData(
                table: "Tokens",
                columns: new[] { "Id", "IsVisible", "Name", "PieceID", "SessionID", "X", "Y", "zIndex" },
                values: new object[,]
                {
                    { 1, true, "Default Dungeon", 2, 1, 0.0, 0.0, 0 },
                    { 2, true, "Goblin Chief", 3, 1, 50.0, 5.0, 1 },
                    { 3, false, "Basic Chest", 4, 1, 50.0, 10.0, 2 },
                    { 4, true, "Cleric", 1, 1, 50.0, 15.0, 3 },
                    { 5, true, "Cleric", 1, 1, 50.0, 20.0, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Pieces_SetID",
                table: "Pieces",
                column: "SetID");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PieceId",
                table: "Purchases",
                column: "PieceId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_UserId",
                table: "Purchases",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pieces_Sets_SetID",
                table: "Pieces",
                column: "SetID",
                principalTable: "Sets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Sessions_SessionID",
                table: "Tokens",
                column: "SessionID",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
