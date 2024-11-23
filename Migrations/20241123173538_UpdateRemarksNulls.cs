using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KASCFlightLogging.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRemarksNulls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1",
                column: "ConcurrencyStamp",
                value: "e3b40e90-e81b-4431-bd06-21de42dcadde");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "29cea22d-8390-4634-bc46-8cba38972411", "AQAAAAIAAYagAAAAEARnJ/OmBu+kcIcr20XYGGDiX7J4e3EV0fzteWGGs45OnBAG/ixumfUCgpMoR99aAg==", "bd88bdac-b61f-4727-bba6-581930130a2d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
