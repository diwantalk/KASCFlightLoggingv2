using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class AddFlightLogReviewFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "d29b4030-ee77-4e9e-8dbb-56784203fc40");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "93fa9173-dd3d-4cf7-8edc-8b2856472986", "AQAAAAIAAYagAAAAEJV/dXYtg7dl5oR99BU2NvKDVpNiSQ1D0xPqp3CAunTT+3KxdRZjDax1xvZjwF7m1A==", "09e542a3-fc04-4906-8b8b-8921cc642ab8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
