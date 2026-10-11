using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddsAlbumLinkedOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 887840109L, "0", 2, true, 1158, "ExportOnWrite", 1, null, 55 });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[] { 1187, "Linked On", -1, "", true, "LinkedOnModel", 1160, 1187 });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 472305031L, "", 12, true, 1187, "LinkAsEmbed", 3, null, 54 },
                    { 1280448088L, "", 15, true, 1187, "LinkIconUrl", 2, null, 54 },
                    { 1838160992L, "", 15, true, 1187, "PublishedUrl", 1, null, 54 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 472305031L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 887840109L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1280448088L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1838160992L);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1187);
        }
    }
}
