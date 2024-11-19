using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class RolesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "49000331-a03e-4efb-b676-8a48574a4338");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "Role", "SecurityStamp" },
                values: new object[] { "534715ce-97e1-450d-870d-3dcde510ce13", "AQAAAAIAAYagAAAAEIN7dHNRHERHL/CUfW4slIxuj/qF0dPq+JiNocjeBXzqnDcQrY9EmBwTXz8iW50BKA==", 0, "aa6f6709-ba44-4e78-9e0c-bfbc2688bfc7" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "545d7f95-54ef-49c3-be0b-22abd3bc4d46");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fe624c4e-3913-447b-912c-a1579529a660", "AQAAAAIAAYagAAAAEMG+xFI6gsrTbomS9TI4r7cIOATSJ1pSrAFb1p+rUDEkFX1Ehq/XufvLM2SwP04tjA==", "6e2c463e-2f5c-49da-ab5e-e8138f74f437" });
        }
    }
}
