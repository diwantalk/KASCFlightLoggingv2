using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class FlightLogFieldAddFieldType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FieldType",
                table: "FlightLogFields",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "67430181-8198-465f-8f30-c5f4916a19b6");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ab727e26-603f-4192-b55a-968b7502cc33", "AQAAAAIAAYagAAAAEN8uvPJzL7UjLu7AolkiGET0C5l7z4yhkcL5YJ3aPeRg1tXgn4aHRQ739/kMywsgcA==", "70a21632-7180-456d-a588-c3b1b78c0fa7" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "FlightLogFields");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "dbe8a04f-0724-4780-805c-8d332048381b");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8be4464a-8ca0-48e1-bf47-e8658d292cc8", "AQAAAAIAAYagAAAAEAlPuU5iv6aKWmWRdi8/xIejFBnCZis/WDDagJwgxjWjv8mctRCH/c0qjQnXuteTyw==", "630d3e4f-5afb-41fe-8501-4e1eb03106c2" });
        }
    }
}
