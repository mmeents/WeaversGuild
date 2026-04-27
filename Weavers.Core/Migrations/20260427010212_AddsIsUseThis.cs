using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddsIsUseThis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "DefaultValue", "EditorTypeId", "ItemTypeId", "Key", "Rank", "ValueDataTypeId" },
                values: new object[] { "0", 2, 526, "UseThis", 4, 1 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "DefaultValue", "Key", "Rank" },
                values: new object[] { ".cs", "FileExtension", 12 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "Key", "Rank" },
                values: new object[] { "FilePath", 13 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "EditorTypeId", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 4, 610, "Namespace", 14, null, 6 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "EditorTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 10, "ImportObject", 2, 500, 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "DefaultValue", "EditorTypeId", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { "", 2, 611, "UseInterface", 3, null, 1 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "DefaultValue", "EditorTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { "64", 10, "DataType", 1, 50, 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "Key", "Rank" },
                values: new object[] { "IsNullable", 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "DefaultValue", "Key", "Rank" },
                values: new object[] { "1", "HasSetter", 4 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "Key", "Rank" },
                values: new object[] { "HasNavigation", 5 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "DefaultValue", "EditorTypeId", "Key", "Rank", "ValueDataTypeId" },
                values: new object[] { "0", 2, "IsPrimaryKey", 6, 1 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "DefaultValue", "EditorTypeId", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId" },
                values: new object[] { "-1", 3, 612, "MaxSize", 7, null });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "DefaultValue", "Key", "Rank", "ReferenceItemTypeId" },
                values: new object[] { "", "ClassType", 2, 610 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "EditorTypeId", "Key", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 10, "HasNavigation", 6, 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "Key", "Rank" },
                values: new object[] { "IsNullable", 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "DefaultValue", "EditorTypeId", "ItemTypeId", "Key", "Rank", "ValueDataTypeId" },
                values: new object[] { "1", 2, 614, "IsCollection", 4, 1 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "DefaultValue", "Key", "Rank" },
                values: new object[] { ".cs", "FileExtension", 12 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "Key", "Rank" },
                values: new object[] { "FilePath", 13 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "EditorTypeId", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 4, 620, "Namespace", 14, null, 6 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "DefaultValue", "EditorTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { "", 10, "ClassType", 2, 612, 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "DefaultValue", "EditorTypeId", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { "1", 2, 622, "IsNullable", 3, null, 1 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "DefaultValue", "EditorTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { "", 10, "ClassType", 2, 614, 3 });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 87, "1", 2, true, 624, "IsNullable", 3, null, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 65,
                columns: new[] { "DefaultValue", "EditorTypeId", "ItemTypeId", "Key", "Rank", "ValueDataTypeId" },
                values: new object[] { ".cs", 4, 610, "FileExtension", 12, 6 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 66,
                columns: new[] { "DefaultValue", "Key", "Rank" },
                values: new object[] { "", "FilePath", 13 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 67,
                columns: new[] { "Key", "Rank" },
                values: new object[] { "Namespace", 14 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 68,
                columns: new[] { "EditorTypeId", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 10, 611, "ImportObject", 2, 500, 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 69,
                columns: new[] { "EditorTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 2, "UseInterface", 3, null, 1 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 70,
                columns: new[] { "DefaultValue", "EditorTypeId", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { "64", 10, 612, "DataType", 1, 50, 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 71,
                columns: new[] { "DefaultValue", "EditorTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { "1", 2, "IsNullable", 3, null, 1 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 72,
                columns: new[] { "Key", "Rank" },
                values: new object[] { "HasSetter", 4 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 73,
                columns: new[] { "DefaultValue", "Key", "Rank" },
                values: new object[] { "0", "HasNavigation", 5 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 74,
                columns: new[] { "Key", "Rank" },
                values: new object[] { "IsPrimaryKey", 6 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 75,
                columns: new[] { "DefaultValue", "EditorTypeId", "Key", "Rank", "ValueDataTypeId" },
                values: new object[] { "-1", 3, "MaxSize", 7, 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 76,
                columns: new[] { "DefaultValue", "EditorTypeId", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId" },
                values: new object[] { "", 10, 614, "ClassType", 2, 610 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 77,
                columns: new[] { "DefaultValue", "Key", "Rank", "ReferenceItemTypeId" },
                values: new object[] { "1", "HasNavigation", 3, 6 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 78,
                columns: new[] { "EditorTypeId", "Key", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 2, "IsNullable", null, 1 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 79,
                columns: new[] { "Key", "Rank" },
                values: new object[] { "IsCollection", 4 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 80,
                columns: new[] { "DefaultValue", "EditorTypeId", "ItemTypeId", "Key", "Rank", "ValueDataTypeId" },
                values: new object[] { ".cs", 4, 620, "FileExtension", 12, 6 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 81,
                columns: new[] { "DefaultValue", "Key", "Rank" },
                values: new object[] { "", "FilePath", 13 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 82,
                columns: new[] { "Key", "Rank" },
                values: new object[] { "Namespace", 14 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 83,
                columns: new[] { "EditorTypeId", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 10, 622, "ClassType", 2, 612, 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 84,
                columns: new[] { "DefaultValue", "EditorTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { "1", 2, "IsNullable", 3, null, 1 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 85,
                columns: new[] { "DefaultValue", "EditorTypeId", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { "", 10, 624, "ClassType", 2, 614, 3 });

            migrationBuilder.UpdateData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 86,
                columns: new[] { "DefaultValue", "EditorTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { "1", 2, "IsNullable", 3, null, 1 });
        }
    }
}
