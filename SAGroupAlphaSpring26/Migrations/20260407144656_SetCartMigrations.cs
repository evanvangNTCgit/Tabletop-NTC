using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class SetCartMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItemSet_Sets_SetId",
                table: "CartItemSet");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItemSet_Users_UserId",
                table: "CartItemSet");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItemSet",
                table: "CartItemSet");

            migrationBuilder.RenameTable(
                name: "CartItemSet",
                newName: "CartItemSets");

            migrationBuilder.RenameIndex(
                name: "IX_CartItemSet_UserId",
                table: "CartItemSets",
                newName: "IX_CartItemSets_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItemSet_SetId",
                table: "CartItemSets",
                newName: "IX_CartItemSets_SetId");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Sets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItemSets",
                table: "CartItemSets",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 7, 9, 46, 56, 10, DateTimeKind.Local).AddTicks(4780));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 7, 9, 46, 56, 10, DateTimeKind.Local).AddTicks(4787));

            migrationBuilder.UpdateData(
                table: "Sets",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsArchived",
                value: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIzne0/mg1aI4lyyM8Xg4Vr+gfEYIT/aP8GeP3Qc+7mom4rDrIVHM4nqQEiushzi6g==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEAyPaITFylwfXRsGYKK3WuAFf0SaInBEeP4UMouvEqEj6EQfDOaq8DRdTDG3cbGfsQ==");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemSets_Sets_SetId",
                table: "CartItemSets",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemSets_Users_UserId",
                table: "CartItemSets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItemSets_Sets_SetId",
                table: "CartItemSets");

            migrationBuilder.DropForeignKey(
                name: "FK_CartItemSets_Users_UserId",
                table: "CartItemSets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CartItemSets",
                table: "CartItemSets");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Sets");

            migrationBuilder.RenameTable(
                name: "CartItemSets",
                newName: "CartItemSet");

            migrationBuilder.RenameIndex(
                name: "IX_CartItemSets_UserId",
                table: "CartItemSet",
                newName: "IX_CartItemSet_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CartItemSets_SetId",
                table: "CartItemSet",
                newName: "IX_CartItemSet_SetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CartItemSet",
                table: "CartItemSet",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemSet_Sets_SetId",
                table: "CartItemSet",
                column: "SetId",
                principalTable: "Sets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartItemSet_Users_UserId",
                table: "CartItemSet",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
