using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFlightLogNullableFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "7a0077dd-4c6b-4d79-9ebf-03ded37149bb");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b4799c51-17c1-4ef5-8582-9c0ab8db3330", "AQAAAAIAAYagAAAAELwUBl261b1Gvx0hWRL2DjHHwYywcMTkVMnoENcgVAquesCId4os4rcFVY4T7zWgfw==", "96605d4a-60d1-4784-b673-5ccce019b783" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "52456256-292d-43a6-a00d-a38c2b22a71d");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c377a281-9c0a-4149-b15a-dc3d5be08d3f", "AQAAAAIAAYagAAAAEOzszyRxDTSOr1V368ZBZ370mkg//2KBXRYzkUGsoEQw3H+sjDy4zWM5sET6k0v0LA==", "0e2a188a-1d38-4a55-99ef-cfb8f855de4e" });
        }
    }
}
