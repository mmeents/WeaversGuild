using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class Db160x2AddsLineLeases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppPeers_HarnessAppSessionId",
                table: "AppPeers");

            migrationBuilder.AddColumn<byte[]>(
                name: "TokenHash",
                table: "AppPeers",
                type: "varbinary(32)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LineLeases",
                columns: table => new
                {
                    Machine = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Line = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    HarnessAppSessionId = table.Column<int>(type: "int", nullable: false),
                    AcquiredAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineLeases", x => x.Machine);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPeers_HarnessAppSessionId",
                table: "AppPeers",
                column: "HarnessAppSessionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineLeases");

            migrationBuilder.DropIndex(
                name: "IX_AppPeers_HarnessAppSessionId",
                table: "AppPeers");

            migrationBuilder.DropColumn(
                name: "TokenHash",
                table: "AppPeers");

            migrationBuilder.CreateIndex(
                name: "IX_AppPeers_HarnessAppSessionId",
                table: "AppPeers",
                column: "HarnessAppSessionId");
        }
    }
}
