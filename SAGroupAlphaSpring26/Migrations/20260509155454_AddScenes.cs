using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class AddScenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SceneId",
                table: "Tokens",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Scenes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false)
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

            migrationBuilder.InsertData(
                table: "Scenes",
                columns: new[] { "Id", "Name", "SessionId" },
                values: new object[,]
                {
                    { 1, "Default Scene", 1 },
                    { 2, "Default Scene", 2 }
                });

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 5, 9, 10, 54, 54, 533, DateTimeKind.Local).AddTicks(7885));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 5, 9, 10, 54, 54, 533, DateTimeKind.Local).AddTicks(7888));

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 1,
                column: "SceneId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 2,
                column: "SceneId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 3,
                column: "SceneId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 4,
                column: "SceneId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 5,
                column: "SceneId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 6,
                column: "SceneId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 7,
                column: "SceneId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 8,
                column: "SceneId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "Id",
                keyValue: 9,
                column: "SceneId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMh17itRoID7c04fW8Iz++qPG74Sj2fv7wPsgQWufgjrUSdVAJyGl781H1nogblt0g==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAENWwR3QfQGWAHiuAfagKqb/aNnH+vhCohlvpSemJb709xlo+vjEzNop9plZf5PqL2g==");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_SceneId",
                table: "Tokens",
                column: "SceneId");

            migrationBuilder.CreateIndex(
                name: "IX_Scenes_SessionId",
                table: "Scenes",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tokens_Scenes_SceneId",
                table: "Tokens",
                column: "SceneId",
                principalTable: "Scenes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tokens_Scenes_SceneId",
                table: "Tokens");

            migrationBuilder.DropTable(
                name: "Scenes");

            migrationBuilder.DropIndex(
                name: "IX_Tokens_SceneId",
                table: "Tokens");

            migrationBuilder.DropColumn(
                name: "SceneId",
                table: "Tokens");

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 5, 7, 12, 16, 47, 115, DateTimeKind.Local).AddTicks(9608));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 5, 7, 12, 16, 47, 115, DateTimeKind.Local).AddTicks(9611));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIW+K67zU6Ji0uPMuCcD56GQr//D+5XmzRvk72Y7sRaGluj1O3qZ2vwA2ZC16kA1Qw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDaAFoQ9Nsa8JQvwEA80Q7zcumT1haPeMhUYES6cIoOPugDW6TihF/rQz731+V37Zw==");
        }
    }
}
