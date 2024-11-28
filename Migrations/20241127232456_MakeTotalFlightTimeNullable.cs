using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class MakeTotalFlightTimeNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TotalFlightTime",
                table: "FlightLogs",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "449883bd-31a2-4177-86f0-319cbddbcbf4");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ac73f818-4eee-4ef4-96fa-3f9a9a3a224b", "AQAAAAIAAYagAAAAECrllgkBuKOEhqvBxXgn7OocGugZVIyX00TOKW8AOF6ua/LM0lHD7AIX8uHWn1/Azw==", "aa8c1612-011d-46fb-80e6-1834179d6b48" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TotalFlightTime",
                table: "FlightLogs",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

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
    }
}
