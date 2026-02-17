using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedFixedSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 2, 17, 16, 23, 44, 913, DateTimeKind.Local).AddTicks(1827));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 17, 16, 23, 44, 913, DateTimeKind.Local).AddTicks(1787));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Sessions",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastUpdated",
                value: new DateTime(2026, 2, 17, 15, 52, 19, 283, DateTimeKind.Local).AddTicks(5154));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 17, 15, 52, 19, 283, DateTimeKind.Local).AddTicks(5084));
        }
    }
}
