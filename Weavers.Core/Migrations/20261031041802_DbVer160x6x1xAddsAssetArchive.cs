using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class DbVer160x6x1xAddsAssetArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaAssets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileHash = table.Column<byte[]>(type: "binary(32)", fixedLength: true, maxLength: 32, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    MediaType = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    CapturedLocal = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    CapturedOffsetMin = table.Column<short>(type: "smallint", nullable: true),
                    CapturedSource = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    Width = table.Column<int>(type: "int", nullable: true),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Orientation = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)1),
                    DurationMs = table.Column<int>(type: "int", nullable: true),
                    CameraModel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Lat = table.Column<double>(type: "float", nullable: true),
                    Lon = table.Column<double>(type: "float", nullable: true),
                    PHash = table.Column<long>(type: "bigint", nullable: true),
                    GroupKey = table.Column<string>(type: "varchar(128)", unicode: false, maxLength: 128, nullable: true),
                    MediaTags = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false, collation: "Latin1_General_100_CI_AS_SC_UTF8")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArchiveAssets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArchiveId = table.Column<int>(type: "int", nullable: false),
                    MediaAssetId = table.Column<int>(type: "int", nullable: false),
                    RelPath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    FileMtimeUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    LastSeenUtc = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    LastVerifiedUtc = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveAssets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchiveAssets_Items_ArchiveId",
                        column: x => x.ArchiveId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArchiveAssets_MediaAssets_MediaAssetId",
                        column: x => x.MediaAssetId,
                        principalTable: "MediaAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 330, "Archive Kinds", -1, "", true, "ArchiveKinds", null, 23 },
                    { 1145, "AssetArchiveModel", -1, "", true, "AssetArchiveModel", 1145, 1145 },
                    { 1146, "OffloadPolicyModel", -1, "", true, "OffloadPolicyModel", 1146, 1146 },
                    { 1147, "ReplicaPolicyModel", -1, "", true, "ReplicaPolicyModel", 1147, 1147 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 269364213L, "100GB", 4, true, 1146, "BitBudget", 12, null, 54 },
                    { 337964304L, "332", 10, true, 1145, "ArchiveKind", 12, 330, 57 },
                    { 391742229L, "", 10, true, 1145, "ServiceHost", 6, 1010, 57 },
                    { 420463839L, "", 13, true, 1145, "RootFolder", 10, null, 54 },
                    { 1235587993L, "0", 2, true, 1145, "Available", 14, null, 55 },
                    { 1371214361L, "", 10, true, 1146, "TargetArchive", 8, 1145, 57 },
                    { 1686085612L, "90", 3, true, 1146, "MinAgeDays", 10, null, 57 },
                    { 1794673994L, "", 4, true, 1145, "VolumeName", 8, null, 54 },
                    { 1836933056L, "", 10, true, 1147, "ReplicaArchive", 8, 1145, 57 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 331, "iCloud Sync Folder", -1, "", true, "AkICloudSync", 330, 1 },
                    { 332, "Archive", -1, "", true, "AkArchive", 330, 2 },
                    { 333, "Replica", -1, "", true, "AkReplica", 330, 3 },
                    { 334, "Removable", -1, "", true, "AkRemovable", 330, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveAssets_Archive_Status",
                table: "ArchiveAssets",
                columns: new[] { "ArchiveId", "Status" })
                .Annotation("SqlServer:Include", new[] { "MediaAssetId", "LastVerifiedUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveAssets_MediaAsset",
                table: "ArchiveAssets",
                column: "MediaAssetId")
                .Annotation("SqlServer:Include", new[] { "ArchiveId", "Status" });

            migrationBuilder.CreateIndex(
                name: "UX_ArchiveAssets_Archive_RelPath",
                table: "ArchiveAssets",
                columns: new[] { "ArchiveId", "RelPath" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_CapturedLocal",
                table: "MediaAssets",
                column: "CapturedLocal");

            migrationBuilder.CreateIndex(
                name: "IX_MediaAssets_GroupKey",
                table: "MediaAssets",
                column: "GroupKey",
                filter: "[GroupKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UX_MediaAssets_FileHash",
                table: "MediaAssets",
                column: "FileHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArchiveAssets");

            migrationBuilder.DropTable(
                name: "MediaAssets");

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 269364213L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 337964304L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 391742229L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 420463839L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1235587993L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1371214361L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1686085612L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1794673994L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1836933056L);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 331);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 332);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 333);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 334);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 330);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1145);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1146);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1147);
        }
    }
}
