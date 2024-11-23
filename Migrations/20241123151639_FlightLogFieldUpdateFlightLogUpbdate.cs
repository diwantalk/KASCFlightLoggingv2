using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class FlightLogFieldUpdateFlightLogUpbdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "dbe8a04f-0724-4780-805c-8d332048381b");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8be4464a-8ca0-48e1-bf47-e8658d292cc8", "AQAAAAIAAYagAAAAEAlPuU5iv6aKWmWRdi8/xIejFBnCZis/WDDagJwgxjWjv8mctRCH/c0qjQnXuteTyw==", "630d3e4f-5afb-41fe-8501-4e1eb03106c2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "204eff63-f0d0-45b8-b0e3-93ab7fcd58f2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b33cd85a-7e6a-4e47-8242-28650153c4e1", "AQAAAAIAAYagAAAAEFZ+PCUjrR125FAOUpvQypIZ5CiVk8YqfcU2vslutTzMq57xOek0YcGG957j/DGSZg==", "676a4ead-fc87-4c47-b5ed-e6b99c5f93d4" });
        }
    }
}
