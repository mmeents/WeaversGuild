using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class DbVer160x6x4xAssetPlanAdditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DoneUtc",
                table: "AssetPlans",
                type: "datetime2(0)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlanSummary",
                table: "AssetPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResultSummary",
                table: "AssetPlans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedUtc",
                table: "AssetPlans",
                type: "datetime2(0)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoneUtc",
                table: "AssetPlans");

            migrationBuilder.DropColumn(
                name: "PlanSummary",
                table: "AssetPlans");

            migrationBuilder.DropColumn(
                name: "ResultSummary",
                table: "AssetPlans");

            migrationBuilder.DropColumn(
                name: "StartedUtc",
                table: "AssetPlans");
        }
    }
}
