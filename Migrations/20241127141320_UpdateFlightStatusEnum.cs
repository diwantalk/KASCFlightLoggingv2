using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFlightStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "100aa7e5-f059-453e-a9b3-3c0f2c779c05");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "442c99b7-4050-4fdd-9d64-84cb71f70105", "AQAAAAIAAYagAAAAEIoEiTb/xB7gtMpztZu3zu2ip7iaja6/E9emGNXPRJMRrKND/nvT5gIUfgFeLlk6Sg==", "fa7fed7e-02c3-4be5-8ca9-78e3b7b83118" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "36375a93-5fdf-4189-afb2-89534968cd65");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0955ccbb-ffdb-4360-a851-ea781711f4df", "AQAAAAIAAYagAAAAEPlfggjLncqIk8I4GVCJ9eHXaiuAnAQFr68bl2x4/5LQyvo4GeFml+WFJV+Gm57qOA==", "a65c2c33-f0cd-4021-b307-519d4f7b53b5" });
        }
    }
}
