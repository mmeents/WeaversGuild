using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddsAppPeersTableForSwarms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPeers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Machine = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HarnessAppSessionId = table.Column<int>(type: "int", nullable: false),
                    ProcessNum = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    HeartBeat = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPeers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPeers_Items_HarnessAppSessionId",
                        column: x => x.HarnessAppSessionId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPeers_HarnessAppSessionId",
                table: "AppPeers",
                column: "HarnessAppSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPeers");
        }
    }
}
