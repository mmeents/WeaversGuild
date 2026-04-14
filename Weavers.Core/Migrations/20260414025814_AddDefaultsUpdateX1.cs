using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultsUpdateX1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 3,
                column: "EditorTypeId",
                value: 14);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 3,
                column: "EditorTypeId",
                value: 5);
        }
    }
}
