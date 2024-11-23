using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    public partial class MakeRemarksNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop any default constraint on Remarks column if it exists
            migrationBuilder.Sql(
                @"DECLARE @ConstraintName nvarchar(200)
                SELECT @ConstraintName = Name FROM SYS.DEFAULT_CONSTRAINTS
                WHERE PARENT_OBJECT_ID = OBJECT_ID('FlightLogs')
                AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns
                                      WHERE NAME = N'Remarks'
                                      AND object_id = OBJECT_ID(N'FlightLogs'))
                IF @ConstraintName IS NOT NULL
                    EXECUTE('ALTER TABLE FlightLogs DROP CONSTRAINT ' + @ConstraintName)");

            // Make Remarks column nullable
            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "FlightLogs",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Make Remarks column non-nullable with empty string default
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
        }
    }
}