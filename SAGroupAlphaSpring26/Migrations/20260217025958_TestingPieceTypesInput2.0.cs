using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SAGroupAlphaSpring26.Migrations
{
    /// <inheritdoc />
    public partial class TestingPieceTypesInput20 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "PieceTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Enemy" },
                    { 2, "Map" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PieceTypes",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
