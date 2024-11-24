using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class MakeRemarksNullableInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                value: "e3b40e90-e81b-4431-bd06-21de42dcadde");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "29cea22d-8390-4634-bc46-8cba38972411", "AQAAAAIAAYagAAAAEARnJ/OmBu+kcIcr20XYGGDiX7J4e3EV0fzteWGGs45OnBAG/ixumfUCgpMoR99aAg==", "bd88bdac-b61f-4727-bba6-581930130a2d" });
        }
    }
}
