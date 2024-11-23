using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDbcontextFlightLogNullableField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "719c1764-d691-45e1-936f-63ae895ea472");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "86b1fb65-c294-4df4-ab8c-74093e9fc898", "AQAAAAIAAYagAAAAEOmZ9LChnZ+B0uW/EawmWSGRqJa2IkOyOepwo9NnVr1xFV0P7RMiPmrVkfLFDDcn5A==", "8b198d6b-3067-4d86-abb3-fbf9c15124dd" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
