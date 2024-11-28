using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFlightLogValueCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightLogValues_FlightLogs_FlightLogId",
                table: "FlightLogValues");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "b61befd1-590b-4cd3-9ef7-68051a57d412");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c929852a-e846-4a0d-b6a0-7ef1e77ff56f", "AQAAAAIAAYagAAAAEO82vl2YWIOUN9hHhGLFd9g8F9NTHVeySMqasIkvQs5dx6TT6u+77E7ze+xKxz6seg==", "7d6c32c9-ce09-4a86-95d0-76cfc450427a" });

            migrationBuilder.AddForeignKey(
                name: "FK_FlightLogValues_FlightLogs_FlightLogId",
                table: "FlightLogValues",
                column: "FlightLogId",
                principalTable: "FlightLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightLogValues_FlightLogs_FlightLogId",
                table: "FlightLogValues");

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

            migrationBuilder.AddForeignKey(
                name: "FK_FlightLogValues_FlightLogs_FlightLogId",
                table: "FlightLogValues",
                column: "FlightLogId",
                principalTable: "FlightLogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
