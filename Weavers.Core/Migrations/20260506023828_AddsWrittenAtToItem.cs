using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddsWrittenAtToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "WrittenAt",
                table: "Items",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WrittenAt",
                table: "Items");
        }
    }
}
