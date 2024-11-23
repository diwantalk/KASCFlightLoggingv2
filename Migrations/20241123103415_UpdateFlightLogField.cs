using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFlightLogField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "Aircraft",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "Aircraft",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

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
    }
}
