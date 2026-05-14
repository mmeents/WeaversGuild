using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddsMcpLogEntryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "McpLogEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CalledAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OpName = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    InputJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OutputJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationMs = table.Column<int>(type: "int", nullable: false),
                    Success = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMsg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_McpLogEntries", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "McpLogEntries");
        }
    }
}
