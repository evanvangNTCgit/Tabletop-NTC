using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PieceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PieceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "usd"),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pieces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    PieceTypeID = table.Column<int>(type: "INTEGER", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pieces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pieces_PieceTypes_PieceTypeID",
                        column: x => x.PieceTypeID,
                        principalTable: "PieceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItemSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SetId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemSets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItemSets_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItemSets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false)
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
                name: "Sessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PieceId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "PieceSets",
                columns: table => new
                {
                    PieceId = table.Column<int>(type: "INTEGER", nullable: false),
                    SetId = table.Column<int>(type: "INTEGER", nullable: false)
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
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PieceId = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "SaleLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SaleID = table.Column<int>(type: "INTEGER", nullable: false),
                    PieceID = table.Column<int>(type: "INTEGER", nullable: false),
                    SetID = table.Column<int>(type: "INTEGER", nullable: true),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Tax = table.Column<decimal>(type: "TEXT", nullable: false),
                    TotalCost = table.Column<decimal>(type: "TEXT", nullable: false)
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
                    table.ForeignKey(
                        name: "FK_SaleLines_Sets_SetID",
                        column: x => x.SetID,
                        principalTable: "Sets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Scenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scenes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scenes_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    PieceID = table.Column<int>(type: "INTEGER", nullable: false),
                    SessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    SceneId = table.Column<int>(type: "INTEGER", nullable: true),
                    X = table.Column<double>(type: "decimal(18,2)", nullable: false),
                    Y = table.Column<double>(type: "decimal(18,2)", nullable: false),
                    ZIndex = table.Column<int>(type: "decimal(18,2)", nullable: false),
                    Visibility = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tokens_Pieces_PieceID",
                        column: x => x.PieceID,
                        principalTable: "Pieces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tokens_Scenes_SceneId",
                        column: x => x.SceneId,
                        principalTable: "Scenes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tokens_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                columns: new[] { "Id", "IsArchived", "Name", "Price" },
                values: new object[] { 1, false, "Evans Beginner Pack", 0.00m });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Currency", "Email", "FirstName", "IsAdmin", "LastName", "PasswordHash" },
                values: new object[,]
                {
                    { 1, "usd", "local@demo.com", "Local", true, "DM", "AQAAAAIAAYagAAAAECqEDkrcNfdbUyFqF53LBQbbNTQoOIfGr0vla7BYRSUEeUNcdo4gcKdrLF6Ou2EskA==" },
                    { 2, "usd", "evankvang@gmail.com", "Evan", false, "Vang", "AQAAAAIAAYagAAAAEHKAAqNfVAmGbRDksGjtXUid6fWJKhEHi7St6RAjSazyNpIx1O0FLgrwjmsA4+tNAw==" }
                });

            migrationBuilder.InsertData(
                table: "Pieces",
                columns: new[] { "Id", "Description", "ImagePath", "IsArchived", "Name", "PieceTypeID", "Price" },
                values: new object[,]
                {
                    { 1, "No description provided", "/images/testMap.png", false, "Default Dungeon", 2, 0.00m },
                    { 2, "No description provided", "/images/Cleric.png", false, "Cleric", 1, 0.00m },
                    { 3, "No description provided", "/images/GoblinChief.png", false, "Goblin Chief", 5, 0.00m },
                    { 4, "No description provided", "/images/chest.png", false, "Basic Chest", 4, 0.00m },
                    { 5, "No description provided", "/images/bardTest.png", false, "Bard", 1, 0.00m },
                    { 6, "No description provided", "/images/BetaMap1.png", false, "Beta Dungeon", 2, 5.00m }
                });

            migrationBuilder.InsertData(
                table: "Sessions",
                columns: new[] { "Id", "LastUpdated", "Name", "Notes", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 3, 18, 9, 57, 880, DateTimeKind.Local).AddTicks(2618), "Test Session", "Local Test Session", 1 },
                    { 2, new DateTime(2026, 6, 3, 18, 9, 57, 880, DateTimeKind.Local).AddTicks(2622), "Test Session 2", "Local Test Session 2", 1 }
                });

            migrationBuilder.InsertData(
                table: "PieceSets",
                columns: new[] { "PieceId", "SetId" },
                values: new object[,]
                {
                    { 2, 1 },
                    { 5, 1 }
                });

            migrationBuilder.InsertData(
                table: "Scenes",
                columns: new[] { "Id", "Name", "SessionId" },
                values: new object[,]
                {
                    { 1, "Default Scene", 1 },
                    { 2, "Default Scene", 2 }
                });

            migrationBuilder.InsertData(
                table: "UserPieces",
                columns: new[] { "PieceId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 }
                });

            migrationBuilder.InsertData(
                table: "Tokens",
                columns: new[] { "Id", "Name", "Notes", "PieceID", "SceneId", "SessionId", "Visibility", "X", "Y", "ZIndex" },
                values: new object[,]
                {
                    { 1, "Default Dungeon", "", 1, 1, 1, true, 0.0, 0.0, 0 },
                    { 2, "Cleric", "", 2, 1, 1, true, 50.0, 15.0, 3 },
                    { 3, "Goblin Chief", "", 3, 1, 1, true, 50.0, 5.0, 1 },
                    { 4, "Basic Chest", "", 4, 1, 1, false, 50.0, 10.0, 2 },
                    { 5, "Beta Dungeon", "", 6, 2, 2, true, 0.0, 0.0, 0 },
                    { 6, "Cleric", "", 2, 2, 2, true, 50.0, 15.0, 3 },
                    { 7, "Cleric", "", 2, 2, 2, true, 50.0, 5.0, 1 },
                    { 8, "Cleric", "", 2, 2, 2, true, 50.0, 10.0, 2 },
                    { 9, "Goblin Chief", "", 3, 2, 2, true, 50.0, 5.0, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_PieceId",
                table: "CartItems",
                column: "PieceId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_UserId",
                table: "CartItems",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemSets_SetId",
                table: "CartItemSets",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemSets_UserId",
                table: "CartItemSets",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pieces_PieceTypeID",
                table: "Pieces",
                column: "PieceTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PieceSets_SetId",
                table: "PieceSets",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleLines_PieceID",
                table: "SaleLines",
                column: "PieceID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleLines_SaleID",
                table: "SaleLines",
                column: "SaleID");

            migrationBuilder.CreateIndex(
                name: "IX_SaleLines_SetID",
                table: "SaleLines",
                column: "SetID");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_UserID",
                table: "Sales",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Scenes_SessionId",
                table: "Scenes",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_PieceID",
                table: "Tokens",
                column: "PieceID");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_SceneId",
                table: "Tokens",
                column: "SceneId");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_SessionId",
                table: "Tokens",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPieces_UserId",
                table: "UserPieces",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "CartItemSets");

            migrationBuilder.DropTable(
                name: "PieceSets");

            migrationBuilder.DropTable(
                name: "SaleLines");

            migrationBuilder.DropTable(
                name: "Tokens");

            migrationBuilder.DropTable(
                name: "UserPieces");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "Sets");

            migrationBuilder.DropTable(
                name: "Scenes");

            migrationBuilder.DropTable(
                name: "Pieces");

            migrationBuilder.DropTable(
                name: "Sessions");

            migrationBuilder.DropTable(
                name: "PieceTypes");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
