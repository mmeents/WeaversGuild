using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddsMusicAlbums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 941576447L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 2140676322L);

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 906081851L, "", 15, true, 1158, "PublishedUrl", 2, null, 54 },
                    { 1389682802L, "0", 2, true, 1160, "ExportOnWrite", 3, null, 55 },
                    { 1672982413L, "0", 2, true, 1180, "ExportOnWrite", 1, null, 55 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[] { 1185, "Music Album", -1, "", true, "MusicAlbumModel", 1160, 1185 });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 1756118225L, "0", 2, true, 1185, "ExportOnWrite", 1, null, 55 },
                    { 1939950204L, "", 12, true, 1185, "Credits", 9, null, 54 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[] { 1186, "Album Track", -1, "", true, "AlbumTrackModel", 1185, 1186 });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[] { 1038379064L, "", 12, true, 1186, "Credits", 9, null, 54 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 906081851L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1038379064L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1389682802L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1672982413L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1756118225L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1939950204L);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1186);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1185);

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 941576447L, "", 15, true, 1180, "CoverImgUrl", 2, null, 54 },
                    { 2140676322L, "", 15, true, 1160, "CoverImgUrl", 5, null, 54 }
                });
        }
    }
}
