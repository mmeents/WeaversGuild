using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddsMinorDefaultUpdatesX2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 78,
                column: "DefaultValue",
                value: "7");

            migrationBuilder.UpdateData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 614,
                column: "ParentTypeId",
                value: 612);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 78,
                column: "DefaultValue",
                value: "1");

            migrationBuilder.UpdateData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 614,
                column: "ParentTypeId",
                value: 610);
        }
    }
}
