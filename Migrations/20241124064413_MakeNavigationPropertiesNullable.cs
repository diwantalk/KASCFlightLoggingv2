using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class MakeNavigationPropertiesNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "FlightLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "0505c171-3a22-426a-a036-f08387b5f281");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5f520c78-50da-4bca-b8e2-1077130f170d", "AQAAAAIAAYagAAAAEF1oyt4RKQC3nZHkEsGQl+mCLVwZ5H6whPmeN7FyqFOf9qmpR5pBfnukTM/A6zQwsw==", "7ffd9e80-0bba-4d4c-a822-e4f8e87c598c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "FlightLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "82f79931-1529-40e5-9a39-c5bc09c9df23");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eb425416-b1f9-40c8-8600-3e31b0264f39", "AQAAAAIAAYagAAAAEE+VNyNh9IpW6GsiAT7gimAMPRL05F2gkxOQnmH9Q+EHgkEKFRHtVehBHfG5GBoZLw==", "eca59a9b-30af-4e39-a135-a517f2ee415c" });
        }
    }
}
