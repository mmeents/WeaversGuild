using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddsWeaversInitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemRelationTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Relation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemRelationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: -1, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: -1, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "{}"),
                    Established = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_ItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemRelations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    RelationTypeId = table.Column<int>(type: "int", nullable: false),
                    RelatedItemId = table.Column<int>(type: "int", nullable: true),
                    Established = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Rank = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemRelations_ItemRelationTypes_RelationTypeId",
                        column: x => x.RelationTypeId,
                        principalTable: "ItemRelationTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemRelations_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemRelations_Items_RelatedItemId",
                        column: x => x.RelatedItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ItemRelationTypes",
                columns: new[] { "Id", "Description", "Relation" },
                values: new object[,]
                {
                    { 200, "Idea → Idea / Idea → Requirement", "INSPIRES" },
                    { 201, "Requirement → Idea", "ADDRESSES" },
                    { 202, "ResearchFinding → Idea / Requirement", "REFINES" },
                    { 203, "Constraint → Idea / Requirement / Capability", "CONSTRAINS" },
                    { 204, "ResearchFinding / PoC → Requirement / Capability", "VALIDATES" },
                    { 205, "Decision → Constraint", "RESOLVES" },
                    { 206, "Capability → Requirement / Feature", "ENABLES" },
                    { 207, "Constraint → Capability", "LIMITS" },
                    { 208, "Requirement / Feature → Capability", "DEPENDS_ON" },
                    { 209, "Capability → Capability (stack evolution)", "EXTENDS" },
                    { 210, "Previous Layer / Artifact → Next Layer", "BUILDS_ON" },
                    { 211, "Agent → Artifact / Layer / Requirement", "PRODUCES" },
                    { 212, "Agent → ResearchFinding / Capability", "CONSUMES" },
                    { 213, "Orchestrator / Layer → Agent", "ASSIGNS_TO" },
                    { 214, "Agent → Command / Handler", "EXECUTES" },
                    { 215, "Handler → Command / Query", "IMPLEMENTS" },
                    { 216, "Core / MediatR → ClientApp", "EXPOSES_TO" },
                    { 217, "Handler → Command / Query", "HANDLES" },
                    { 218, "Command → Handler", "TRIGGERS" },
                    { 219, "Handler → DomainEntity", "RETURNS" },
                    { 220, "ClientApp → Command / Query", "USES" },
                    { 221, "DomainEntity → DomainEntity (relationships)", "BELONGS_TO" },
                    { 222, "Feature / Requirement → DomainEntity / Handler", "REQUIRES" },
                    { 223, "LessonLearned → Capability / Decision", "INFORMS" },
                    { 224, "PoC / Experiment → LessonLearned / Requirement", "RESULTS_IN" },
                    { 225, "Container → Item (Layer → Feature, etc.)", "CONTAINS" },
                    { 226, "Artifact → Requirement / Idea", "DERIVES_FROM" },
                    { 227, "Loose reference between any two items", "REFERENCES" },
                    { 228, "Sequencing: Layer A precedes Layer B", "PRECEDES" },
                    { 229, "Sequencing: Layer B follows Layer A", "FOLLOWS" }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 100, "Raw product ideas or chicken-and-egg problems (starting point)", "Idea" },
                    { 101, "What the stack, agents, or developer can currently do reliably", "Capability" },
                    { 102, "Scope, tech, time, or client-type limitations", "Constraint" },
                    { 103, "Functional or non-functional specifications / user stories", "Requirement" },
                    { 104, "Market, competitor, feasibility, or validation insights", "ResearchFinding" },
                    { 105, "Business domain object in the app (e.g. Task, Record)", "DomainEntity" },
                    { 106, "MediatR command (e.g. CreateTaskCommand)", "Command" },
                    { 107, "MediatR query", "Query" },
                    { 108, "MediatR command/query handler", "Handler" },
                    { 109, "WinFormsClient, AngularClient, ConsoleTester, etc.", "ClientApp" },
                    { 110, "AI agent role (ResearchAgent, LayerBuilderAgent, etc.)", "Agent" },
                    { 111, "Build layer in the agent chain (RequirementsLayer, InfraLayer, etc.)", "Layer" },
                    { 112, "Scoped feature or user story", "Feature" },
                    { 113, "Architectural or scoping decision with rationale", "Decision" },
                    { 114, "Post-iteration insights and capability updates", "LessonLearned" },
                    { 115, "Generated output (code snippet, diagram, mock, document)", "Artifact" },
                    { 116, "Proof-of-concept experiment or vertical slice", "PoC" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemRelations_ItemId_RelationTypeId_RelatedItemId",
                table: "ItemRelations",
                columns: new[] { "ItemId", "RelationTypeId", "RelatedItemId" },
                unique: true,
                filter: "[RelatedItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRelations_RelatedItemId",
                table: "ItemRelations",
                column: "RelatedItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRelations_RelationTypeId",
                table: "ItemRelations",
                column: "RelationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemRelationTypes_Relation",
                table: "ItemRelationTypes",
                column: "Relation",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemTypeId",
                table: "Items",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTypes_Name",
                table: "ItemTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemRelations");

            migrationBuilder.DropTable(
                name: "ItemRelationTypes");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "ItemTypes");
        }
    }
}
