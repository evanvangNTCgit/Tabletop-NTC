using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class MakeSessionNotesNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 25, 14, 58, 6, 804, DateTimeKind.Local).AddTicks(7719));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 4, 25, 14, 58, 6, 804, DateTimeKind.Local).AddTicks(7723));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKlUfZW0M90IVWp+rS8P741RjV0eTwyqv3VMrlUSmWydJ33AVt02GStShKlKhTU1qg==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAELQQXzDxRORX+ouA3P+HMhgr6Lih9Mgk7H5AhwM/WH4UOkTHZaxp3rCO4bvsSh8S8g==");
        }
    }
}
