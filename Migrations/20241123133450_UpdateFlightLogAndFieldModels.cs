using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFlightLogAndFieldModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "204eff63-f0d0-45b8-b0e3-93ab7fcd58f2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b33cd85a-7e6a-4e47-8242-28650153c4e1", "AQAAAAIAAYagAAAAEFZ+PCUjrR125FAOUpvQypIZ5CiVk8YqfcU2vslutTzMq57xOek0YcGG957j/DGSZg==", "676a4ead-fc87-4c47-b5ed-e6b99c5f93d4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "7126457a-cab4-40b9-8722-0e06dbd91e4b");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f9df5156-f8fa-4b52-ae5f-c33160d9d2fb", "AQAAAAIAAYagAAAAEAYPVlMEOhrbdkBxBStSMVbF+CyrjqFiNJi6pzlCcWw+AV3q0RvB4JFw31mDh8MXfw==", "994be0b9-dfe2-48bc-9c37-9de0c85d165b" });
        }
    }
}
