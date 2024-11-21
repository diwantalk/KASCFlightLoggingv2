using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAircraft : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "72c5a5c3-e61c-4266-b173-0eeb33f60f7c");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "126d0e76-f5a5-4674-91b1-ee11e22dfcad", "AQAAAAIAAYagAAAAEGe/dMQUz+Lev7lPCzlAsysKFgEfFYo2GurduoOA4WJikoGn+Nzj8QT67ZDoTwKriQ==", "1fa142cd-a192-45f8-a1d4-64c010f76c9c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "32549c4f-9c63-47f6-8b92-5bb11d41e6a8");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f79061e8-c413-4c6b-88ab-6e33001438b4", "AQAAAAIAAYagAAAAEDyXT4sf4oIG6sa252exdHLbb5Q/JDVrYuthP/74C9ib5GeSXwJUrC2pgEMM3Rj5PA==", "5ab90a70-a883-4373-9f21-5d92856f5603" });
        }
    }
}
