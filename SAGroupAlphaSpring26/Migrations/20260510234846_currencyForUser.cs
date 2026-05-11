using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class currencyForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "usd");

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 5, 10, 18, 48, 45, 588, DateTimeKind.Local).AddTicks(5073));

            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastUpdated",
                value: new DateTime(2026, 5, 10, 18, 48, 45, 588, DateTimeKind.Local).AddTicks(5078));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Currency", "PasswordHash" },
                values: new object[] { "usd", "AQAAAAIAAYagAAAAEIhKJBYXkJawtvygYWDCdzn65iVm0EgH7mXwccKuCVbD/rpCob4eFxNF4wzLwH80rA==" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Currency", "PasswordHash" },
                values: new object[] { "usd", "AQAAAAIAAYagAAAAEIbUSYTJaRCrwoox4Cy9PUi83pQBimulE0PEyirdKmVn6fBsj1Ipzurv3k3nC7DoXw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Users");

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
        }
    }
}
