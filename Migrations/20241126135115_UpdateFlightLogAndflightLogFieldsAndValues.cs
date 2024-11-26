using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFlightLogAndflightLogFieldsAndValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalLocation",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "ArrivalTime",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "DepartureLocation",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "DepartureTime",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "NumberOfLandings",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "PassengerCount",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "TotalTime",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "FlightLogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "FlightLogs",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "FlightLogs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FlightLogs",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "FlightLogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedById",
                table: "FlightLogs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PilotId",
                table: "FlightLogs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PublishedById",
                table: "FlightLogs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TotalFlightTime",
                table: "FlightLogs",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<int>(
                name: "FieldType",
                table: "FlightLogFields",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "36375a93-5fdf-4189-afb2-89534968cd65");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0955ccbb-ffdb-4360-a851-ea781711f4df", "AQAAAAIAAYagAAAAEPlfggjLncqIk8I4GVCJ9eHXaiuAnAQFr68bl2x4/5LQyvo4GeFml+WFJV+Gm57qOA==", "a65c2c33-f0cd-4021-b307-519d4f7b53b5" });

            migrationBuilder.CreateIndex(
                name: "IX_FlightLogs_ApplicationUserId",
                table: "FlightLogs",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightLogs_ModifiedById",
                table: "FlightLogs",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_FlightLogs_PilotId",
                table: "FlightLogs",
                column: "PilotId");

            migrationBuilder.CreateIndex(
                name: "IX_FlightLogs_PublishedById",
                table: "FlightLogs",
                column: "PublishedById");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightLogs_AspNetUsers_ApplicationUserId",
                table: "FlightLogs",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlightLogs_AspNetUsers_ModifiedById",
                table: "FlightLogs",
                column: "ModifiedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightLogs_AspNetUsers_PilotId",
                table: "FlightLogs",
                column: "PilotId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlightLogs_AspNetUsers_PublishedById",
                table: "FlightLogs",
                column: "PublishedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlightLogs_AspNetUsers_ApplicationUserId",
                table: "FlightLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightLogs_AspNetUsers_ModifiedById",
                table: "FlightLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightLogs_AspNetUsers_PilotId",
                table: "FlightLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_FlightLogs_AspNetUsers_PublishedById",
                table: "FlightLogs");

            migrationBuilder.DropIndex(
                name: "IX_FlightLogs_ApplicationUserId",
                table: "FlightLogs");

            migrationBuilder.DropIndex(
                name: "IX_FlightLogs_ModifiedById",
                table: "FlightLogs");

            migrationBuilder.DropIndex(
                name: "IX_FlightLogs_PilotId",
                table: "FlightLogs");

            migrationBuilder.DropIndex(
                name: "IX_FlightLogs_PublishedById",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "PilotId",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "PublishedById",
                table: "FlightLogs");

            migrationBuilder.DropColumn(
                name: "TotalFlightTime",
                table: "FlightLogs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "FlightLogs",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "ArrivalLocation",
                table: "FlightLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalTime",
                table: "FlightLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartureLocation",
                table: "FlightLogs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DepartureTime",
                table: "FlightLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NumberOfLandings",
                table: "FlightLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassengerCount",
                table: "FlightLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "FlightLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TotalTime",
                table: "FlightLogs",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "FlightLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "FieldType",
                table: "FlightLogFields",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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
    }
}
