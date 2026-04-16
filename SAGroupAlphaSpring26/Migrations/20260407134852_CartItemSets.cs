using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class CartItemSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartItemSet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SetId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItemSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItemSet_Sets_SetId",
                        column: x => x.SetId,
                        principalTable: "Sets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItemSet_Users_UserId",
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
                value: new DateTime(2026, 4, 7, 8, 48, 51, 603, DateTimeKind.Local).AddTicks(4571));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 7, 8, 48, 51, 603, DateTimeKind.Local).AddTicks(4575));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHHEvViQWY5KxLpO0te7UcdBXMR9XoHeBQa06+vzDUMxZS0gTe42L2kI/rzwJWLFww==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKi+qia6UmdkzqcPBYqokY8upSUKTIGavIPMbY7C27bn5awQ4XYoapnaTbkQKKkrKQ==");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemSet_SetId",
                table: "CartItemSet",
                column: "SetId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItemSet_UserId",
                table: "CartItemSet",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItemSet");

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 2, 12, 35, 58, 27, DateTimeKind.Local).AddTicks(6634));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 2, 12, 35, 58, 27, DateTimeKind.Local).AddTicks(6638));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFoxFEJ+GOAvTcNaIY3evf7abKMUdg/Kom2gBi06zN7fcRTNjvEZNQEVpyIk6Ag8Qw==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJJjbDNQjjVcEm72d7tvKYB52cYuIM58UIZp9jzFTGHqgJa3Wr+TtBMUFbj/NNi9fQ==");
        }
    }
}
