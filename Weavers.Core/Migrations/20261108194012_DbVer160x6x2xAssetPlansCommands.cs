using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class DbVer160x6x2xAssetPlansCommands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RelPath",
                table: "ArchiveAssets",
                type: "nvarchar(800)",
                maxLength: 800,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);

            migrationBuilder.CreateTable(
                name: "AssetPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyItemId = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    CommandCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Bytes = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ApprovedUtc = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetPlans_Items_PolicyItemId",
                        column: x => x.PolicyItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetCommands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetPlanId = table.Column<int>(type: "int", nullable: false),
                    Kind = table.Column<byte>(type: "tinyint", nullable: false),
                    Status = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    Step = table.Column<byte>(type: "tinyint", nullable: false, defaultValue: (byte)0),
                    MediaAssetId = table.Column<int>(type: "int", nullable: false),
                    Bytes = table.Column<long>(type: "bigint", nullable: false),
                    SourceHarnessId = table.Column<int>(type: "int", nullable: false),
                    SourceArchiveId = table.Column<int>(type: "int", nullable: false),
                    SourceArchiveAssetId = table.Column<int>(type: "int", nullable: false),
                    TargetHarnessId = table.Column<int>(type: "int", nullable: false),
                    TargetArchiveId = table.Column<int>(type: "int", nullable: false),
                    TargetRelPath = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    TargetArchiveAssetId = table.Column<int>(type: "int", nullable: true),
                    ClaimedBy = table.Column<string>(type: "varchar(64)", unicode: false, maxLength: 64, nullable: true),
                    LeaseUntilUtc = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    NotBeforeUtc = table.Column<DateTime>(type: "datetime2(0)", nullable: true),
                    Attempts = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2(0)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    DoneUtc = table.Column<DateTime>(type: "datetime2(0)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetCommands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetCommands_ArchiveAssets_SourceArchiveAssetId",
                        column: x => x.SourceArchiveAssetId,
                        principalTable: "ArchiveAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetCommands_ArchiveAssets_TargetArchiveAssetId",
                        column: x => x.TargetArchiveAssetId,
                        principalTable: "ArchiveAssets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AssetCommands_AssetPlans_AssetPlanId",
                        column: x => x.AssetPlanId,
                        principalTable: "AssetPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetCommands_Items_SourceArchiveId",
                        column: x => x.SourceArchiveId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetCommands_Items_SourceHarnessId",
                        column: x => x.SourceHarnessId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetCommands_Items_TargetArchiveId",
                        column: x => x.TargetArchiveId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetCommands_Items_TargetHarnessId",
                        column: x => x.TargetHarnessId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AssetCommands_MediaAssets_MediaAssetId",
                        column: x => x.MediaAssetId,
                        principalTable: "MediaAssets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetCommands_Claim",
                table: "AssetCommands",
                columns: new[] { "TargetHarnessId", "Status", "NotBeforeUtc" })
                .Annotation("SqlServer:Include", new[] { "LeaseUntilUtc", "Attempts" });

            migrationBuilder.CreateIndex(
                name: "IX_AssetCommands_MediaAssetId",
                table: "AssetCommands",
                column: "MediaAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCommands_Plan_Status",
                table: "AssetCommands",
                columns: new[] { "AssetPlanId", "Status" })
                .Annotation("SqlServer:Include", new[] { "Bytes" });

            migrationBuilder.CreateIndex(
                name: "IX_AssetCommands_SourceArchiveAssetId",
                table: "AssetCommands",
                column: "SourceArchiveAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCommands_SourceArchiveId",
                table: "AssetCommands",
                column: "SourceArchiveId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCommands_SourceHarnessId",
                table: "AssetCommands",
                column: "SourceHarnessId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCommands_TargetArchiveAssetId",
                table: "AssetCommands",
                column: "TargetArchiveAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetCommands_TargetArchiveId",
                table: "AssetCommands",
                column: "TargetArchiveId");

            migrationBuilder.CreateIndex(
                name: "UX_AssetCommands_Plan_MediaAsset",
                table: "AssetCommands",
                columns: new[] { "AssetPlanId", "MediaAssetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetPlans_Policy_Status",
                table: "AssetPlans",
                columns: new[] { "PolicyItemId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetCommands");

            migrationBuilder.DropTable(
                name: "AssetPlans");

            migrationBuilder.AlterColumn<string>(
                name: "RelPath",
                table: "ArchiveAssets",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(800)",
                oldMaxLength: 800);
        }
    }
}
