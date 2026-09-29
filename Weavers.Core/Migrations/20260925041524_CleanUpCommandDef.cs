using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class CleanUpCommandDef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CommandDefs",
                keyColumn: "Id",
                keyValue: 461);

            migrationBuilder.DeleteData(
                table: "CommandDefs",
                keyColumn: "Id",
                keyValue: 465);

            migrationBuilder.UpdateData(
                table: "CommandDefs",
                keyColumn: "Id",
                keyValue: 469,
                column: "Description",
                value: "Adds a new Comfy Todo. Clones the workflow template.");

            migrationBuilder.InsertData(
                table: "CommandDefs",
                columns: new[] { "Id", "Code", "Description", "Group", "LegacyId", "McpCode" },
                values: new object[] { 462, "CmdListComfyWorkflows", "Lists all Comfy workflow templates installed.", "comfy", 1, "listComfyWorkflows" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CommandDefs",
                keyColumn: "Id",
                keyValue: 462);

            migrationBuilder.UpdateData(
                table: "CommandDefs",
                keyColumn: "Id",
                keyValue: 469,
                column: "Description",
                value: "Adds a new todo to a Comfy workflow.");

            migrationBuilder.InsertData(
                table: "CommandDefs",
                columns: new[] { "Id", "Code", "Description", "Group", "LegacyId", "McpCode" },
                values: new object[,]
                {
                    { 461, "CmdAddComfyWorkflow", "Adds a new Comfy workflow.", "comfy", 1, "addComfyWorkflow" },
                    { 465, "CmdAddComfyWfParam", "Adds a new parameter to a Comfy workflow.", "comfy", 1, "addComfyWfParam" }
                });
        }
    }
}
