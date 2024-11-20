using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class FlightLogFieldUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRequired",
                table: "FlightLogFields",
                newName: "Required");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "FlightLogFields",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Aircraft",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "FlightLogFields");

            migrationBuilder.RenameColumn(
                name: "Required",
                table: "FlightLogFields",
                newName: "IsRequired");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Aircraft",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

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
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "534715ce-97e1-450d-870d-3dcde510ce13", "AQAAAAIAAYagAAAAEIN7dHNRHERHL/CUfW4slIxuj/qF0dPq+JiNocjeBXzqnDcQrY9EmBwTXz8iW50BKA==", "aa6f6709-ba44-4e78-9e0c-bfbc2688bfc7" });
        }
    }
}
