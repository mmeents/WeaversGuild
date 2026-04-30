using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddsMinorDefaultUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 80,
                column: "DefaultValue",
                value: "0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 80,
                column: "DefaultValue",
                value: "1");
        }
    }
}
