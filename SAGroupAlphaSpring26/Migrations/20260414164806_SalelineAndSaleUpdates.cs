using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class SalelineAndSaleUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SetID",
                table: "SaleLines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Tax",
                table: "SaleLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "SaleLines",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 14, 11, 48, 5, 721, DateTimeKind.Local).AddTicks(1127));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 14, 11, 48, 5, 721, DateTimeKind.Local).AddTicks(1131));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOo4mhuDm2R7rOsGbN9glHmkDUMZIeM+e2OHLgDPzcVBI2x7V3GMOF4uEr2FLLAN1w==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAED/gmvbkG61Zn3N16R4ClGqB85/9PxJ3Db+XsqA+LzgNWyUGiv+Uvf1uvRX65CjTzA==");

            migrationBuilder.CreateIndex(
                name: "IX_SaleLines_SetID",
                table: "SaleLines",
                column: "SetID");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleLines_Sets_SetID",
                table: "SaleLines",
                column: "SetID",
                principalTable: "Sets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleLines_Sets_SetID",
                table: "SaleLines");

            migrationBuilder.DropIndex(
                name: "IX_SaleLines_SetID",
                table: "SaleLines");

            migrationBuilder.DropColumn(
                name: "SetID",
                table: "SaleLines");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "SaleLines");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "SaleLines");

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
        }
    }
}
