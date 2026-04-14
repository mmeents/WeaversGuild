using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultsUpdateX2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 2,
                column: "DefaultValue",
                value: "");

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 16,
                column: "EditorTypeId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 20,
                column: "EditorTypeId",
                value: 10);

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "EditorTypeId", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 10, 420, 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 24,
                column: "EditorTypeId",
                value: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 2,
                column: "DefaultValue",
                value: "1");

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 16,
                column: "EditorTypeId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 20,
                column: "EditorTypeId",
                value: 11);

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "EditorTypeId", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 4, null, 6 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 24,
                column: "EditorTypeId",
                value: 11);
        }
    }
}
