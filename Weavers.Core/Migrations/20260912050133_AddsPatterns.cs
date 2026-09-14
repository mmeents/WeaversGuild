using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Weavers.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddsPatterns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[,]
                {
                    { 205, "addPattern", -1, "", true, "CmdAddPattern", 120, 72 },
                    { 206, "addPatDimension", -1, "", true, "CmdAddPatDimension", 120, 73 },
                    { 207, "addPatDimOption", -1, "", true, "CmdAddPatDimOption", 120, 74 },
                    { 208, "getNextDraw", -1, "", true, "CmdGetNextDraw", 120, 75 },
                    { 209, "rejectDraw", -1, "", true, "CmdRejectDraw", 120, 76 },
                    { 210, "acceptDraw", -1, "", true, "CmdAcceptDraw", 120, 77 },
                    { 310, "Draw Status", -1, "", true, "DrawStatus", null, 21 },
                    { 1090, "Pattern", -1, "", true, "PatternModel", 1000, 1090 },
                    { 311, "Draw Issued", -1, "", true, "DrawIssued", 310, 1 },
                    { 312, "Draw Declined", -1, "", true, "DrawDeclined", 310, 2 },
                    { 313, "Draw Written", -1, "", true, "DrawWritten", 310, 3 },
                    { 314, "Draw Accepted", -1, "", true, "DrawAccepted", 310, 4 },
                    { 315, "Draw Rejected", -1, "", true, "DrawRejected", 310, 5 },
                    { 1092, "Pattern Dimension", -1, "", true, "PatternDimensionModel", 1090, 1092 },
                    { 1096, "Pattern Draw", -1, "", true, "PatternDrawModel", 1090, 1096 }
                });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 47605316L, "", 3, true, 1092, "Rank", 10, null, 57 },
                    { 1135556740L, "311", 10, true, 1096, "DrawStatus", 2, 310, 57 },
                    { 2081888173L, "", 4, true, 1096, "AddedBy", 1, null, 54 }
                });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "Id", "Description", "EditorTypeId", "IconName", "IsVisible", "Name", "ParentTypeId", "Rank" },
                values: new object[] { 1094, "Pattern Option", -1, "", true, "PatternOptionModel", 1092, 1094 });

            migrationBuilder.InsertData(
                table: "ItemPropertyDefaults",
                columns: new[] { "Id", "DefaultValue", "EditorTypeId", "IsVisible", "ItemTypeId", "Key", "Rank", "ReferenceItemTypeId", "ValueDataTypeId" },
                values: new object[,]
                {
                    { 129223570L, "", 3, true, 1094, "Rank", 10, null, 57 },
                    { 365055096L, "0", 3, true, 1094, "IssuedCount", 3, null, 57 },
                    { 481157527L, "0", 3, true, 1094, "AcceptedCount", 2, null, 57 },
                    { 563161507L, "0", 3, true, 1094, "RejectedCount", 1, null, 57 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 47605316L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 129223570L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 365055096L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 481157527L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 563161507L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 1135556740L);

            migrationBuilder.DeleteData(
                table: "ItemPropertyDefaults",
                keyColumn: "Id",
                keyValue: 2081888173L);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 205);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 206);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 207);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 208);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 209);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 210);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 311);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 312);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 313);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 314);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 315);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 310);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1094);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1096);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1092);

            migrationBuilder.DeleteData(
                table: "ItemTypes",
                keyColumn: "Id",
                keyValue: 1090);
        }
    }
}
